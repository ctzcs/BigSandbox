using System;
using RVO;
using UnityEngine;
using Random = System.Random;
using Simulator = RVO2.Scripts.RVO.Simulator;
using Vector2 = RVO.Vector2;

namespace RVO2.Scripts
{
    public class GameAgent : MonoBehaviour
    {
        [HideInInspector] public int sid = -1;

        /** Random number generator. */
        private Random m_random = new Random();

        private Transform _transform;
        // Use this for initialization
        void Start()
        {
            _transform = this.transform;
        }

        // Update is called once per frame
        void Update()
        {
            if (sid >= 0)
            {
                Vector2 pos = Simulator.Instance.getAgentPosition(sid);
                Vector2 vel = Simulator.Instance.getAgentPrefVelocity(sid);
                var targetPos = new Vector3(pos.x(),pos.y(), 0);
                MoveTo(targetPos,10);



                /*if (Math.Abs(vel.x()) > 0.01f && Math.Abs(vel.y()) > 0.01f)
                //以Y轴为移动朝向
                transform.up = new Vector3(vel.x(),vel.y(), 0 ).normalized;*/
            }

            ////如果没有按下右键的话, 就不给予速度向量
            if (!Input.GetMouseButton(1))
            {
                Simulator.Instance.setAgentPrefVelocity(sid, new Vector2(0, 0));
                return;
            }

            Vector2 goalVector = GameMainManager.Instance.mousePosition - Simulator.Instance.getAgentPosition(sid);
            if (RVOMath.absSq(goalVector) > 1.0f)
            {
                goalVector = RVOMath.normalize(goalVector);
            }

            Simulator.Instance.setAgentPrefVelocity(sid, goalVector);

            /* Perturb a little to avoid deadlocks due to perfect symmetry. */
            /* 因为完美对称，所以需要加入些许抖动用来避免死锁
         * 这里的完美对称，测试出是指两个完全一样的单位，不抖动=中心点一样=无法把其他排斥出去
         */
            float angle = (float) m_random.NextDouble()*2.0f*(float) Math.PI;
            float dist = (float) m_random.NextDouble()*0.0001f;

            Simulator.Instance.setAgentPrefVelocity(sid, Simulator.Instance.getAgentPrefVelocity(sid) +
                                                         dist*
                                                         new Vector2((float) Math.Cos(angle), (float) Math.Sin(angle)));
        }

        void MoveTo(Vector3 target,float speed)
        {
            if (Math.Abs(target.x) >= 0 )
            {
                _transform.position = Vector3.Lerp(_transform.position, target, Time.deltaTime*speed);
            }
        }
    }
}