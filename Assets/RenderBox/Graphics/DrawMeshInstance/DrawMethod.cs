using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;


namespace RenderBox.Graphics
{
    public class DrawMethod : MonoBehaviour
    {
        public enum EDrawType
        {
            RenderInstance,
            RenderIndirect,
        }
        [Header("公共")]
        public Vector3 left;
        public Vector3 right;
        private const int m_Count = 20000;
        public Random random;
        public Mesh mesh;
        public Matrix4x4[] newMats;
        public EDrawType type = EDrawType.RenderInstance;
        
        
        [Header("instance")]
        public Material material;
        /// <summary>
        /// 命令的数量
        /// </summary>
        private int m_CommandCountPer1024;
        
        [Header("Indirect")]
        //Indirect
        public Material indirectMaterial;
        private GraphicsBuffer m_GraphicsBuffer;
        private GraphicsBuffer m_TransformBuffer;
        /// <summary>
        /// 有几种需要渲染的网格/材质
        /// </summary>
        private GraphicsBuffer.IndirectDrawIndexedArgs[] m_CommandData;
        private static readonly int m_ObjectToWorld = Shader.PropertyToID("_ObjectToWorld");
        private static readonly int m_PropertyIDMatrices = Shader.PropertyToID("_Matrices");
        private float m_FixedTime = 0.016f;
        private float m_ElapsedTime = 0;
        void Start()
        {
            left = Camera.main.ViewportToWorldPoint(Vector2.zero);
            left.z = 0;
            right = Camera.main.ViewportToWorldPoint(Vector2.one);
            right.z = 0;
            
            random = Random.CreateFromIndex(1000);
            
            newMats = new Matrix4x4[m_Count];
            m_CommandCountPer1024 = m_Count / 1000;
            for (int i = 0; i < m_Count; i++)
            {
                var pos = random.NextFloat3(left, right).xyz;
                pos.z = 0;
                newMats[i] = Matrix4x4.TRS(pos, Quaternion.identity, Vector3.one);
            }
            
            //要传给GPU的数据
            m_TransformBuffer =  new GraphicsBuffer(GraphicsBuffer.Target.Structured, m_Count, 4 * 4 * sizeof(float));
            m_CommandData = new GraphicsBuffer.IndirectDrawIndexedArgs[1];
            m_GraphicsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, m_CommandData.Length,
                GraphicsBuffer.IndirectDrawIndexedArgs.size);
            
        }
        private void Update()
        {
            //RenderIndirect();
            switch(type)
            {
                case EDrawType.RenderIndirect:
                    RenderIndirect();
                    break;
                case EDrawType.RenderInstance: 
                    RenderInstance();
                    break;
            }
            
        }
        

        // Start is called before the first frame update
        



        void RenderInstance()
        {
            RenderParams rp = new RenderParams(material);
            if (m_ElapsedTime > m_FixedTime)
            {
                for (int i = 0; i < m_Count / 1000; i++)
                {
                    int index = i * 1000;

                    for (int j = 0; j < 1000; j++)
                    {
                        var deltaPos =
                            random.NextFloat2(new float2(-0.1f, -0.1f), new float2(0.1f, 0.1f))
                                .xy; 
                        var column = newMats[index + j].GetColumn(3);
                        float3 newPos = new float3(column[0] + deltaPos.x, column[1] + deltaPos.y,0);
                        newPos.xy = math.clamp(newPos.xy, new float2(left.x, left.y), new float2(right.x, right.y));
                        /*UnityEngine.Graphics.DrawTexture(new Rect(newPos,new float2(texture.width/10,texture.height/10)),texture);*/
                        newMats[index + j] = Matrix4x4.TRS(newPos, Quaternion.identity, Vector3.one);//.SetTRS(newPos,Quaternion.identity,Vector3.one);
                        //newMats[index + j] = Matrix4x4.Translate(new Vector3(newPos.x, newPos.y, 0));
                    }
                    
                    UnityEngine.Graphics.RenderMeshInstanced(rp, mesh, 0, newMats, 1000, index);
                }

                m_ElapsedTime = 0;
            }
            else
            {
                for (int i = 0; i < m_Count / 1000; i++)
                {
                    int index = i * 1000;
                    //UnityEngine.Graphics.RenderMeshIndirect(rp,mesh,new GraphicsBuffer());
                    UnityEngine.Graphics.RenderMeshInstanced(rp, mesh, 0, newMats, 1000, index);
                }
            }

            m_ElapsedTime += Time.deltaTime;

        }

        void RenderIndirect()
        {
            //新的Render主要就是设置RenderParams   
            RenderParams rp = new RenderParams(indirectMaterial);
            //FOV剔除
            rp.worldBounds = new Bounds(Vector3.zero, 10000 * Vector3.one);
                
            rp.matProps = new MaterialPropertyBlock();
            //只有这时候才改坐标
            /*if (m_ElapsedTime > m_FixedTime)
            {*/
                for (int i = 0; i < m_Count; i++)
                {
                    var deltaPos =
                        UnityEngine.Random.insideUnitCircle;
                        //random.NextFloat3(new float3(-0.1f, -0.1f, 0), new float3(0.1f, 0.1f, 0)).xy; //new float2(0.01f, 0.01f); //
                    var column = newMats[i].GetColumn(3);
                    float3 newPos = new float3(column[0] + deltaPos.x, column[1] + deltaPos.y, 0);
                    newPos.x = Mathf.Clamp(newPos.x,left.x,right.x);
                    newPos.y = Mathf.Clamp(newPos.y, left.y, right.y);//math.clamp(newPos.xy, new float2(left.x, left.y), new float2(right.x, right.y));
                    //newMats[i].SetColumn(3,new float4(newPos,1));
                    newMats[i] = Matrix4x4.TRS(newPos, Quaternion.identity, Vector3.one);//.SetTRS(newPos, Quaternion.identity, Vector3.one);
                }
                /*
                m_ElapsedTime = 0;
            }
            m_ElapsedTime += Time.deltaTime;*/

            m_CommandData[0].indexCountPerInstance = mesh.GetIndexCount(0);
            m_CommandData[0].instanceCount = m_Count;
            
            m_TransformBuffer.SetData(newMats);
            //rp.matProps.SetBuffer(PropertyID_Matrices,m_TransformBuffer);
            rp.material.SetBuffer(m_PropertyIDMatrices,m_TransformBuffer); 
            
            m_GraphicsBuffer.SetData(m_CommandData);
            UnityEngine.Graphics.RenderMeshIndirect(rp, mesh, m_GraphicsBuffer,m_CommandData.Length);
        }


        private void OnDestroy()
        {
            m_TransformBuffer?.Release();
            m_TransformBuffer = null;
            m_GraphicsBuffer?.Release();
            m_GraphicsBuffer = null;
        }
    }
}
