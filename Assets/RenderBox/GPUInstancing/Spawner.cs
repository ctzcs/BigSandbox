using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Box1.GPUInstancing
{
    
    public class Spawner:MonoBehaviour
    {
        public enum EMethod
        {
            Default,
            EnableInstancing,
        }

        public EnableInstancing gpuInstancing;
        public int count = 100;
        public EMethod nowMethod;
        public EMethod lastMethod;
        public GameObject prefabs;
        private MeshRenderer _renderer;
        private Material _material;
        private bool _dirty = false;
        public List<GameObject> objs = new List<GameObject>();

        public List<MeshRenderer> renderers = new List<MeshRenderer>();
        private void Start()
        {
            _renderer = prefabs.GetComponent<MeshRenderer>();
            _material = _renderer.sharedMaterial;
            gpuInstancing = GetComponent<EnableInstancing>();
        }

        private void Update()
        {
            if (!_dirty)
                return;
            //运行重新创建物体
            DestroyAll();
            Create();
            SetClean();
        }
        
        void SetMethod(EMethod method)
        {
            nowMethod = method;
            _dirty = true;
        }

        void SetClean()
        {
            _dirty = false;
            if (nowMethod != lastMethod)
            {
                lastMethod = nowMethod;
            }
        }

        void Create()
        {
            gpuInstancing.SetClose();
            switch (nowMethod)
            {
                case EMethod.Default:
                    DefaultCreate();
                    break;
                case EMethod.EnableInstancing:
                    gpuInstancing.Create();
                    break;
            }
        }

        void DefaultCreate()
        {
            for (int i = 0; i < count; i++)
            {
                for (int j = 0; j < count; j++)
                {
                    var pos = new Vector3(i,0,j);
                    var obj = Instantiate(prefabs);
                    obj.transform.localPosition = pos;
                    
                    var r = obj.GetComponent<MeshRenderer>();
                    renderers.Add(r);
                    objs.Add(obj);
                }
            }

            //通过block来设置可更改的属性
            MaterialPropertyBlock prop = new MaterialPropertyBlock();
            for (int i = 0; i < renderers.Count; i++)
            {
                prop = new MaterialPropertyBlock();
                Color color = new Color(Random.Range(0f,1f),Random.Range(0f,1f),Random.Range(0f,1f),1);
                prop.SetColor("_Color",color);
                renderers[i].SetPropertyBlock(prop);
            }
        }

        void DestroyAll()
        {
            if (objs.Count <= 0)
            {
                return;
            }
            for (int i = 0; i < objs.Count; i++)
            {
                Destroy(objs[i]);
            }
            objs.Clear();
            renderers.Clear();
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(100,100,200,200));
            if (GUILayout.Button("Default"))
            {
                SetMethod(EMethod.Default);
            }

            if (GUILayout.Button("EnableInstancing"))
            {
                SetMethod(EMethod.EnableInstancing);
            }
            GUILayout.EndArea();
        }
    }
}