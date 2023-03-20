
using System.Collections.Generic;
using System.Numerics;
using Boids;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace MyFlowField
{
    public class FBoid : MonoBehaviour
    {
        private FBoidData _boidData;
        private List<FBoid> _boidsNeighbor;
        private List<FBoid> _boidsCollRisk;
        private Transform _transform;
        private Rigidbody2D _rb;
        private Vector3 _nowForward;
        private Vector3 _nextForward;
        private Vector3 _nextPosition;//还没用
        private Vector3 _fieldPath;
        
        private float _aggregationFactor = 1f;//凝聚因子
        private float _separationFactor = 1;//分离因子
        private float _convergenceFactor = 1f;//趋同因子
        private float _findPathFactor = 1f;

        private Transform _player;
        #region Property

        public Vector3 NowForward
        {
            get
            {
                return _nowForward;
            }
        }

        #endregion
        private 
        // Start is called before the first frame update
        void Start()
        {
            _transform = this.transform;
            _rb = GetComponent<Rigidbody2D>();
            _boidData = new FBoidData(4, 45, 3);
            _boidsNeighbor = new List<FBoid>();
            _boidsCollRisk = new List<FBoid>();
            _player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        private void FixedUpdate()
        {
            CheckRisk();
            CheckPlayer();
            GetTarget();
            if (_nowForward != _nextForward)
            {
                _rb.velocity = _nextForward;
                _nowForward = _nextForward;
            }
        }

        // Update is called once per frame
        void Update()
        {
            
        }
        

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Monster"))
            {
                Vector3 pos = other.transform.position;
                if (IsValidNeighbor(pos))
                {
                    FBoid boid = other.GetComponent<FBoid>();
                    _boidsNeighbor.Add(boid);
                    if (IsCollRisk(pos))
                    {
                        _boidsCollRisk.Add(boid);
                    }
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            other.TryGetComponent(out FBoid b);
            if (b != null)
            {
                _boidsNeighbor.Remove(b);
            }
        }

        /// <summary>
        /// 检查是否还有Risk
        /// </summary>
        void CheckRisk()
        {
            if (_boidsCollRisk.Count <= 0)
            {
                return;
            }

            for (int i = _boidsCollRisk.Count-1; i >= 0; i--)
            {
                if (IsCollRisk(_boidsCollRisk[i].transform.position))
                {
                    continue;
                }
                _boidsCollRisk.Remove(_boidsCollRisk[i]);
            }

            if (_boidsCollRisk.Count > 1)
            {
                _separationFactor = 2;
                _aggregationFactor = 0;
            }
            else
            {
                _separationFactor = 0f;
                _aggregationFactor = 1;
            }
        }

        void CheckPlayer()
        {
            if (Vector3.Distance(_player.position,_transform.position) > 5)
            {
                _findPathFactor = 3;
            }
            else
            {
                _findPathFactor = 1f;
            }
        }
        /// <summary>
        /// 在规定的视线范围内称为有效邻居
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        bool IsValidNeighbor(Vector3 pos)
        {
            var v = pos - _nowForward;
            if (v.y > 0 && Vector3.Angle(_nowForward,v) <= _boidData.DetectionAngle)
            {
                return true;
            }

            return false;
        }

        bool IsCollRisk(Vector3 pos)
        {
            if (Vector3.Distance(pos,_transform.position) <= _boidData.RiskDistance )
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 用来求平均的方法
        /// </summary>
        void GetTarget()
        {
            Vector3 targetVelocity;
            
            var position = _transform.position;
            Vector3 aggregateVelocity = GetAveragePosition(_boidsNeighbor) - position; //让东西趋近平均点
            var averageVelocity = GetAverageVelocity(_boidsNeighbor);//让东西趋近于平均速度
            var riskCollVelocity = position - GetAveragePosition(_boidsCollRisk);//让东西原理平均点
            targetVelocity = aggregateVelocity.normalized*_aggregationFactor + averageVelocity.normalized * _convergenceFactor + riskCollVelocity.normalized * _separationFactor + _fieldPath.normalized*_findPathFactor;
            _nextForward = targetVelocity.normalized;
        }
        
        //获取 List<Boid>当中 所有Boid 的平均位置
        public Vector3 GetAveragePosition(List<FBoid> someBoids)
        {
            Vector3 sum = Vector3.zero;
            foreach (FBoid b in someBoids)
                sum += b.transform.position;
            Vector3 center = sum / someBoids.Count;
            return (center);
        }

        //获取 List<Boid> 当中 所有Boid 的平均速度
        public Vector3 GetAverageVelocity(List<FBoid> someBoids)
        {
            Vector3 sum = Vector3.zero;
            foreach (FBoid b in someBoids)
                sum += b.NowForward;
            Vector3 avg = sum / someBoids.Count;

            return (avg);
        }

        public void FlowPath(Vector3 dir)
        {
            _fieldPath = dir;
        }
    }
}
