

using System.Collections.Generic;
using DataStructures.ViliWonka.KDTree;
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
            
            for (int i = 0; i < 300 ; i++)
            {
                Debug.Log("生成");
                var obj = Instantiate(prefab);
                var t = obj.transform;
                t.position = new Vector3(Random.Range(rangeMin.x, rangeMax.x), 1, Random.Range(rangeMin.z, rangeMax.z));
                AddToList(t);
            }

        }

        

        public void AddToList(Transform target)
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
            //这里只是利用数据结构去查询位置，真正的Transform在List中，所以这里对KdTree赋值不会产生什么真正的变化。
            //让怪物树中的每一个怪物的坐标更新，这里其实是暗地里把怪物的坐标都给改了。
            for (int i = 0; i < _monsterList.Count; i++)
            {
                _pointCloud[i] = _monsterList[i].position;
            }
            //如果列表的数量小于怪物树的数量，那么将树中剩下的点都放到极限位置
            for (int i = _monsterList.Count; i < _pointCloud.Length; i++)
            {
                _pointCloud[i] = v;
            }
            _monsterTree.Rebuild();
        }

        public void RemoveFromList(Transform target)
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
            UnityEngine.Physics.Raycast(ray, out RaycastHit hitInfo,1000);
            if (hitInfo.transform == null)
            {
                return;
            }

            var pos = new Vector3(hitInfo.point.x, 0, hitInfo.point.z);
            //这里查询到的，正好就是List中的下标
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
                AddToList(t);
            }
            UpdateMonsterTree();
        }
    }

}
