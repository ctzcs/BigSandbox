
using System;
using System.Collections;
using System.Collections.Generic;
using DataStructures.ViliWonka.KDTree;
using MyFlowField;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace KdTree
{
    public class MonsterManager : MonoBehaviour
    {
        private KDTree _monsterTree;
        private Vector3[] _pointCloud;
        private List<Transform> _monsterList;
        private int _monsterCount;
        private List<int> _results;
        
        
        [Header("最小生成点")]
        public Vector3 rangeMin;
        [Header("最大生成点")]
        public Vector3 rangeMax;
        
        //
        public GameObject prefab;
        //帮助计算的私有量
        
        private void Awake()
        {
            BuildTree();
            _results = new List<int>(13);
        }

        public void BuildTree()
        {
            Vector3 v = new Vector3(-10000, -10000, -10000);
            _pointCloud = new Vector3[300];
            for (int i = 0; i < _pointCloud.Length; i++)
            {
                _pointCloud[i] = v;
            }
            
            int maxPointPerLeafNode = 32;
            _monsterTree = new KDTree(_pointCloud, maxPointPerLeafNode);
            
            //生成怪物列表
            _monsterList = new List<Transform>(_pointCloud.Length);
            
            for (int i = 0; i < 10 ; i++)
            {
                Debug.Log("生成");
                var obj = Instantiate(prefab);
                var t = obj.transform;
                t.position = new Vector3(Random.Range(rangeMin.x, rangeMax.x), 0, Random.Range(rangeMin.z, rangeMax.z));
                AddToTree(t);
            }
            
            
        }
        public void AddToTree(Transform target)
        {
            if (_monsterList == null)
                _monsterList = new List<Transform>(_pointCloud.Length);
            _monsterCount++;
            _monsterList.Add(target);
            UpdateMonsterTree();
        }

        void UpdateMonsterTree()
        {
            Vector3 v = new Vector3(-1000, -1000, -1000);
            for (int i = 0; i < _monsterList.Count; i++)
            {
                _pointCloud[i] = _monsterList[i].position;
            }

            for (int i = _monsterList.Count; i < _pointCloud.Length; i++)
            {
                _pointCloud[i] = v;
            }
            _monsterTree.Rebuild();
        }

        public void RemoveFromTree(Entity monster)
        {
            
        }
        

        public void Query()
        {
            _results.Clear();
            KDQuery query = new KDQuery();
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray, out RaycastHit hitInfo);
            if (hitInfo.transform != null)
            {
                var pos = new Vector3(hitInfo.point.x,0, hitInfo.point.z);
                query.Radius(_monsterTree,pos,5,_results);
            }
            for (int i = 0; i < _results.Count; i++)
            {
                Debug.Log(_results[i]);
                if (_monsterList[_results[i]].TryGetComponent(out MeshRenderer m))
                {
                    ChangeColor(m,0.5f);
                }
            }
        }

        void ChangeColor(MeshRenderer m, float delay)
        {
            var mat = m.material;
            if (mat.color == Color.red)
            {
                return;
            }
            StartCoroutine(IEChangeColor(mat, delay));
        }
        IEnumerator IEChangeColor(Material m,float delay)
        {
            var c = m.color;
            m.color = Color.red;
            yield return new WaitForSeconds(delay);
            m.color = c;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Query();
            }

            if (Input.GetMouseButtonDown(1))
            {
                var obj = Instantiate(prefab);
                var t = obj.transform;
                t.position = new Vector3(Random.Range(rangeMin.x, rangeMax.x), 0, Random.Range(rangeMin.z, rangeMax.z));
                AddToTree(t);
            }
        }
    }

}
