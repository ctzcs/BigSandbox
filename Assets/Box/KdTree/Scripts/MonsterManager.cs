
using System;
using System.Collections;
using System.Collections.Generic;
using DataStructures.ViliWonka.KDTree;
using MyFlowField;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering.Universal;
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
            
            for (int i = 0; i < 60 ; i++)
            {
                Debug.Log("生成");
                var obj = Instantiate(prefab);
                var t = obj.transform;
                t.position = new Vector3(Random.Range(rangeMin.x, rangeMax.x), 1, Random.Range(rangeMin.z, rangeMax.z));
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

        public void RemoveFromTree(Transform target)
        {
            _monsterCount--;
            _monsterList.Remove(target);
            UpdateMonsterTree();
        }
        

        public void Query()
        {
            _results.Clear();
            KDQuery query = new KDQuery();
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray, out RaycastHit hitInfo,1000);
            if (hitInfo.transform == null)
            {
                return;
            }

            var pos = new Vector3(hitInfo.point.x, 0, hitInfo.point.z);
            query.Radius(_monsterTree,pos,2,_results);
            
            for (int i = 0; i < _results.Count; i++)
            {
                if (_monsterList[_results[i]].TryGetComponent(out Entity e))
                {
                    //改变颜色0.5s
                    e.ChangeColor(0.5f);
                }
            }
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
                var mousePos = Input.mousePosition;
                mousePos.z = 20;
                t.position = Camera.main.ScreenToWorldPoint(mousePos);
                AddToTree(t);
            }
        }
    }

}
