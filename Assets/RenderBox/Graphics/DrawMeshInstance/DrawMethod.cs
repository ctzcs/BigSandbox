using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.Burst;
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
            RenderIndirectByFloat3,
        }
        [Header("公共")]
        public Vector3 left;
        public Vector3 right;
        [ShowInInspector,SerializeField]
        private uint m_Count = 10000;
        public Random random;
        public Mesh mesh;
        public Matrix4x4[] newPosMatrix;
        public EDrawType type = EDrawType.RenderInstance;
        /// <summary>
        /// 位置数组
        /// </summary>
        public float3[] positions;

        /// <summary>
        /// 颜色数据
        /// </summary>
        public float3[] colors;
        
        [Header("instance")]
        public Material material;
        /// <summary>
        /// 命令的数量
        /// </summary>
        private uint m_CommandCountPer1024;
        
        [Header("Indirect")]
        //Indirect
        public Material indirectMaterial;
        private GraphicsBuffer m_GraphicsBuffer;
        //GraphicsBuffer是一个通用Buffer，可以转换成其他的buffer使用
        private GraphicsBuffer m_TransformBuffer;
        private GraphicsBuffer m_TransformBuffer2;

        private GraphicsBuffer m_ColorBuffer;
        
        /// <summary>
        /// 有几种需要渲染的网格/材质
        /// </summary>
        private GraphicsBuffer.IndirectDrawIndexedArgs[] m_CommandData;
        private static readonly int m_ObjectToWorld = Shader.PropertyToID("_ObjectToWorld");
        private static readonly int m_PropertyIDMatrices = Shader.PropertyToID("_Matrices");
        private float m_FixedTime = 0.016f;
        private float m_ElapsedTime = 0;
        private static readonly int m_Float3Pos = Shader.PropertyToID("_Float3Pos");
        private static readonly int m_Colors = Shader.PropertyToID("_Colors");



        #region Culling

        private CullingGroup _cullingGroup;
        private BoundingSphere[] _bounds;
        private Dictionary<int, Matrix4x4> _cullingMatrices;
        private Matrix4x4[] _drawMatrices;
        private bool _calcDrawFlag = false;
        
        

        #endregion
        private void Awake()
        {
            
            positions = new float3[m_Count];
            colors = new float3[m_Count];
            //要传给GPU的数据
            //位置矩阵使用
            m_TransformBuffer =  new GraphicsBuffer(GraphicsBuffer.Target.Structured, (int)m_Count, 4 * 4 * sizeof(float));
            //float3使用
            m_TransformBuffer2 = new GraphicsBuffer(GraphicsBuffer.Target.Structured, (int)m_Count, 3*sizeof(float));
            //间接渲染参数使用
            m_CommandData = new GraphicsBuffer.IndirectDrawIndexedArgs[1];
            m_GraphicsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, m_CommandData.Length,
                GraphicsBuffer.IndirectDrawIndexedArgs.size);
            m_ColorBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, (int)m_Count, 3*sizeof(float));

            #region Culling模块

            _cullingGroup = new CullingGroup();
            _cullingGroup.targetCamera = Camera.main;
            _cullingGroup.SetDistanceReferencePoint(Camera.main.transform);
            


            #endregion
        }


        void OnChangeRange(CullingGroupEvent ev)
        {
            if (ev.isVisible) _cullingMatrices[ev.index] = newPosMatrix[ev.index];
            else
            {
                _cullingMatrices.Remove(ev.index);
            }

            _calcDrawFlag = true;
        }
        void Start()
        {
            left = Camera.main.ViewportToWorldPoint(Vector2.zero);
            left.z = 0;
            right = Camera.main.ViewportToWorldPoint(Vector2.one);
            right.z = 0;
            
            random = Random.CreateFromIndex(1000);
            
            newPosMatrix = new Matrix4x4[m_Count];
            m_CommandCountPer1024 = m_Count / 1000;


            
            for (int i = 0; i < m_Count; i++)
            {
                var pos = random.NextFloat3(left, right).xyz;
                pos.z = 0;
                newPosMatrix[i] = Matrix4x4.TRS(pos, Quaternion.identity, Vector3.one);
                positions[i] = new float3(pos);
            }
            
            
            
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
                case EDrawType.RenderIndirectByFloat3:
                    RenderIndirectUseFloat3();
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
                        var column = newPosMatrix[index + j].GetColumn(3);
                        float3 newPos = new float3(column[0] + deltaPos.x, column[1] + deltaPos.y,0);
                        newPos.xy = math.clamp(newPos.xy, new float2(left.x, left.y), new float2(right.x, right.y));
                        /*UnityEngine.Graphics.DrawTexture(new Rect(newPos,new float2(texture.width/10,texture.height/10)),texture);*/
                        newPosMatrix[index + j] = Matrix4x4.TRS(newPos, Quaternion.identity, Vector3.one);//.SetTRS(newPos,Quaternion.identity,Vector3.one);
                        //newMats[index + j] = Matrix4x4.Translate(new Vector3(newPos.x, newPos.y, 0));
                    }
                    
                    UnityEngine.Graphics.RenderMeshInstanced(rp, mesh, 0, newPosMatrix, 1000, index);
                }

                m_ElapsedTime = 0;
            }
            else
            {
                for (int i = 0; i < m_Count / 1000; i++)
                {
                    int index = i * 1000;
                    //UnityEngine.Graphics.RenderMeshIndirect(rp,mesh,new GraphicsBuffer());
                    UnityEngine.Graphics.RenderMeshInstanced(rp, mesh, 0, newPosMatrix, 1000, index);
                }
            }

            m_ElapsedTime += Time.deltaTime;

        }

        void RenderIndirect()
        {
            //新的Render主要就是设置RenderParams   
            RenderParams rp = new RenderParams(indirectMaterial);
            //FOV剔除
            rp.worldBounds = new Bounds(Vector3.zero, 100 * Vector3.one);
                
            rp.matProps = new MaterialPropertyBlock();
            //只有这时候才改坐标
            /*if (m_ElapsedTime > m_FixedTime)
            {*/
                for (int i = 0; i < m_Count; i++)
                {
                    var deltaPos =
                        UnityEngine.Random.insideUnitCircle;
                        //random.NextFloat3(new float3(-0.1f, -0.1f, 0), new float3(0.1f, 0.1f, 0)).xy; //new float2(0.01f, 0.01f); //
                    var column = newPosMatrix[i].GetColumn(3);
                    float3 newPos = new float3(column[0] + deltaPos.x, column[1] + deltaPos.y, 0);
                    newPos.x = Mathf.Clamp(newPos.x,left.x,right.x);
                    newPos.y = Mathf.Clamp(newPos.y, left.y, right.y);//
                    //newPos.xy = math.clamp(newPos.xy, new float2(left.x, left.y), new float2(right.x, right.y));//
                    //newMats[i].SetColumn(3,new float4(newPos,1));
                    newPosMatrix[i] = Matrix4x4.TRS(newPos, Quaternion.identity, Vector3.one);//.SetTRS(newPos, Quaternion.identity, Vector3.one);
                    
                    
                }
                /*
                m_ElapsedTime = 0;
            }
            m_ElapsedTime += Time.deltaTime;*/
            //因为可能有不同的网格和材质，可以一起设置
            //每个实例的索引数
            m_CommandData[0].indexCountPerInstance = mesh.GetIndexCount(0);
            //实例的数量
            m_CommandData[0].instanceCount = m_Count;
            //设置坐标buffer
            m_TransformBuffer.SetData(newPosMatrix);
            //将rp Shader中的缓冲区设置为TransformBuffer
            rp.material.SetBuffer(m_PropertyIDMatrices,m_TransformBuffer); 
            //渲染命令队列Buffer
            m_GraphicsBuffer.SetData(m_CommandData);
            
            UnityEngine.Graphics.RenderMeshIndirect(rp, mesh, m_GraphicsBuffer,m_CommandData.Length);
            
            
        }
        [BurstCompile]
        void RenderIndirectUseFloat3()
        {
            //新的Render主要就是设置RenderParams   
            RenderParams rp = new RenderParams(indirectMaterial);
            //FOV剔除
            rp.worldBounds = new Bounds(Vector3.zero, 10000 * Vector3.one);
            //设置MaterialPropertyBlock
            rp.matProps = new MaterialPropertyBlock();
            //只有这时候才改坐标
            /*if (m_ElapsedTime > m_FixedTime)
            {*/
            for (int i = 0; i < m_Count; i++)
            {
                var deltaPos =
                    UnityEngine.Random.insideUnitCircle;
                //random.NextFloat3(new float3(-0.1f, -0.1f, 0), new float3(0.1f, 0.1f, 0)).xy; //new float2(0.01f, 0.01f); //
                /*var column = newMats[i].GetColumn(3);*/
                float3 position = positions[i];
                float3 newPos = new float3(position.x + deltaPos.x, position.y + deltaPos.y, 0);
                float colorX = Mathf.Abs(deltaPos.x);
                float colorY = Mathf.Abs(deltaPos.y);
                float3 color = new float3(colorX,colorY,1);
                
                newPos.x = Mathf.Clamp(newPos.x,left.x,right.x);
                newPos.y = Mathf.Clamp(newPos.y, left.y, right.y);//
                //newPos.xy = math.clamp(newPos.xy, new float2(left.x, left.y), new float2(right.x, right.y));//
                //newMats[i].SetColumn(3,new float4(newPos,1));
                //newMats[i] = Matrix4x4.TRS(newPos, Quaternion.identity, Vector3.one);//.SetTRS(newPos, Quaternion.identity, Vector3.one);
                //将数据保存在Csharp端
                positions[i] = newPos;
                colors[i] = color;
                
            }
            /*
            m_ElapsedTime = 0;
        }
        m_ElapsedTime += Time.deltaTime;*/

            m_CommandData[0].indexCountPerInstance = mesh.GetIndexCount(0);
            m_CommandData[0].instanceCount = m_Count;
            //将Csharp端数据设置到Buffer中
            m_TransformBuffer2.SetData(positions);
            //rp.matProps.SetBuffer(PropertyID_Matrices,m_TransformBuffer);
            m_ColorBuffer.SetData(colors);
            //将Buffer设置到材质的Shader中
            rp.material.SetBuffer(m_Float3Pos,m_TransformBuffer2); 
            rp.material.SetBuffer(m_Colors,m_ColorBuffer);
            m_GraphicsBuffer.SetData(m_CommandData);
            UnityEngine.Graphics.RenderMeshIndirect(rp, mesh, m_GraphicsBuffer,m_CommandData.Length);
        }


        private void OnDestroy()
        {
            m_TransformBuffer?.Release();
            m_TransformBuffer = null;
            
            m_ColorBuffer?.Release();
            m_ColorBuffer = null;
            m_TransformBuffer2.Release();
            m_TransformBuffer2 = null;
            m_GraphicsBuffer?.Release();
            m_GraphicsBuffer = null;
        }
    }
}
