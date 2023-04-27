
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;


namespace Box1.GPUInstancing
{
    public class EnableInstancing:MonoBehaviour
    {
        public Mesh mesh;
        public Material mat;
        public GameObject prefabs;
        public Matrix4x4[] matrix;
        public int instanceCount = 1000;
        
        
        private MeshFilter[] _meshFilter;
        private Renderer[] _renderer;

        ShadowCastingMode  castShadows;

        private bool _turnOnInstance;
        private void Start()
        {
            //注意由于网格和材质必须一样，所以必须使用SharedMaterial
            mesh = prefabs.GetComponent<MeshFilter>().sharedMesh;
            mat = prefabs.GetComponent<MeshRenderer>().sharedMaterial;
            
            matrix = new Matrix4x4[instanceCount];
            
            castShadows = ShadowCastingMode.On;
            _turnOnInstance = false;
            
            
        }

        public void Create()
        {
            for (int i = 0; i < instanceCount; i++)
            {
                
                //pos
                float x = Random.Range(-100, 100);
                float y = 0;
                float z = Random.Range(-100, 100);
                matrix[i] = Matrix4x4.identity;
                matrix[i].SetColumn(3,new Vector4(x,y,z,1));

                /*matrix[i].m00 = Mathf.Max(1, x);
                matrix[i].m11 = Mathf.Max(1, y);
                matrix[i].m22 = Mathf.Max(1, z);*/
            }

            _turnOnInstance = true;
             
        }

        public void SetClose()
        {
            _turnOnInstance = false;
        }

        private void Update()
        {
            
            if (_turnOnInstance&&mesh)
            {
                /*MaterialPropertyBlock prop;
                for (int i = 0; i < matrix.Length; i++)
                {
                    prop = new MaterialPropertyBlock();
                    Color color = new Color(Random.Range(0f,1f),Random.Range(0f,1f),Random.Range(0f,1f),1);
                    prop.SetColor("_Color",color);
                    renderers[i].SetPropertyBlock(prop);
                }*/
                Graphics.DrawMeshInstanced(mesh,0,mat,matrix,matrix.Length);
               
                
            } 
        }
    }
}