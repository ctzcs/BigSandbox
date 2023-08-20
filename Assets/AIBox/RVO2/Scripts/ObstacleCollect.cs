using System.Collections.Generic;
using UnityEngine;
using Simulator = RVO2.Scripts.RVO.Simulator;
using Vector2 = RVO.Vector2;
namespace RVO2.Scripts
{
    /// <summary>
    /// 外置障碍物集合
    /// </summary>
    public class ObstacleCollect : MonoBehaviour
    {
        void Awake()
        {
            BoxCollider[] boxColliders = GetComponentsInChildren<BoxCollider>();
            for (int i = 0; i < boxColliders.Length; i++)
            {
                //這裡修改xy
                float minX = boxColliders[i].transform.position.x -
                             boxColliders[i].size.x*boxColliders[i].transform.lossyScale.x*0.5f;
                float minY = boxColliders[i].transform.position.y -
                             boxColliders[i].size.y*boxColliders[i].transform.lossyScale.y*0.5f;
                float maxX = boxColliders[i].transform.position.x +
                             boxColliders[i].size.x*boxColliders[i].transform.lossyScale.x*0.5f;
                float maxY = boxColliders[i].transform.position.y +
                             boxColliders[i].size.y*boxColliders[i].transform.lossyScale.y*0.5f;

                IList<Vector2> obstacle = new List<Vector2>();
                obstacle.Add(new Vector2(maxX, maxY));
                obstacle.Add(new Vector2(minX, maxY));
                obstacle.Add(new Vector2(minX, minY));
                obstacle.Add(new Vector2(maxX, minY));
                Simulator.Instance.addObstacle(obstacle);
            }
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}