using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Boids
{
    public class BoidsSpawner:MonoBehaviour
    {
        public string PATH = "Prefabs/";
        
        [SerializeField]
        private int m_count = 1;

        [SerializeField] private string loadRes = "Cat_Variant";
        
        private void Start()
        {
            for (int i = 0; i < m_count; i++)
            {
                Spawner();
            }
            
        }

        void Spawner()
        {
            var obj = GetPrefabs(loadRes);
            BoidData data = new BoidData(1f, 0);
            //自定义位置生成怪物
            Vector2 pos = new Vector2(Random.Range(0, 12), Random.Range(0, 12));
            obj = Instantiate(obj,pos,Quaternion.identity);
            obj.AddComponent<Boid>().InitData(data);
        }

        GameObject GetPrefabs(string res)
        {
            return Resources.Load<GameObject>(PATH + res);
        }
    }
}