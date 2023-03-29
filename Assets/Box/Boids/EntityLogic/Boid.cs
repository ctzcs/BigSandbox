
using System.Collections.Generic;
using UnityEngine;
namespace Boids
{
    public class Boid : MonoBehaviour
    {
        private BoidData _boidData = null;
        private int _id;
        private readonly RaycastHit2D[] _hitInfos = new RaycastHit2D[10];

        private Transform _transform;

        private Vector3 _forward;

        private float _timeElapse;
        //运行时数据
        
        private LinkedList<Boid> _neighbors;
        private LinkedList<Boid> _collisionRisks;

        #region Property

        public Vector3 Forward
        {
            get
            {
                return _forward;
            }
        }
        

        #endregion

        #region LifeLoop
        // Start is called before the first frame update
        void Start()
        {
            _transform = transform;
            _forward = _transform.up;
            _neighbors = new LinkedList<Boid>();
            _collisionRisks = new LinkedList<Boid>();
            
        }

        private void OnEnable()
        {
            
        }

        private void FixedUpdate()
        {
            //感觉每帧都检测太消耗性能了
            //似乎可以改成0.4s检测一次
            RemoveInValidNeighbor();
            NeighborsDetect();
            
            
        }

        // Update is called once per frame
        void Update()
        {


        }

        private void OnDisable()
        {
            
            DeRegister();
        }

        #endregion



        #region DetectModule
        /// <summary>
        /// 实例化之后调用的初始化obj的方法
        /// </summary>
        /// <param name="userData"></param>
        public void InitData(object userData)
        {
            if (userData == null)
            {
                return;
            }
            _boidData = userData as BoidData;
            Register();
        }

        /// <summary>
        /// 调用后注册到管理器中
        /// </summary>
        void Register()
        {
            //注册到字典中，其实应该是诞生的时候给予数据并给到字典中，这里为了测试
            _id = this.gameObject.GetInstanceID();
            BoidsManager.Instance.AddToDic(_id,this);
        }

        void DeRegister()
        {
            BoidsManager.Instance.RemoveFromDic(_id);
        }

        /// <summary>
        /// 每帧开始移除无效的邻居
        /// </summary>
        void RemoveInValidNeighbor()
        {
            foreach (var neighbor in _neighbors)
            {
                if (!IsNeighbor(neighbor.transform.position))
                {
                    _neighbors.Remove(neighbor);
                }
            }
        }
        /// <summary>
        /// 使用射线检测邻居
        /// </summary>
        void NeighborsDetect()
        {
            if (_boidData == null)
            {
                return;
            }
            int count = Physics2D.CircleCastNonAlloc(_transform.position,_boidData.DetectionRadius,_transform.up,_hitInfos,_boidData.DetectionLayer);
            if (count <= 1)
            {
                return;
            }
            for (int i = 0; i < count; i++)
            {
                int id = GetRootObjInstanceId(_hitInfos[i]);
                if (id == this._id)
                {
                    continue;
                }
                //从字典中获取已经注册的boid的信息
                Boid b = BoidsManager.Instance.GetFromDic(id);
                if (b == null||_neighbors.Contains(b))
                {
                    return;
                }
                //是不是有效邻居呢？也就是我们规定的这个怪物的视线范围内的邻居
                if (IsValidNeighbor(b.transform.position))
                {
                    _neighbors.AddLast(b);
                }
                
                //判断是否有碰撞危险，如果有加入危险list
                
            }
            //
            CollisionRisk(_neighbors);
        }

        void CollisionRisk(LinkedList<Boid> neighbors)
        {
            if (neighbors.Count > 0)
            {
                foreach (var b in neighbors)
                {
                    //print("找到了" + b.m_id);
                }
            }
        }

        
        /// <summary>
        /// 一帧一世界，帧结束，清空缓存中的世界
        /// </summary>
        void TickClear()
        {
            
        }

        /// <summary>
        /// 用来检测是不是仍是邻居
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        bool IsNeighbor(Vector3 pos)
        {
            if (Vector3.Distance(pos,_transform.position) > _boidData.DetectionRadius)
            {
                return false;
            }

            return IsValidNeighbor(pos);
             

        }
        /// <summary>
        /// 在规定的视线范围内称为有效邻居
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        bool IsValidNeighbor(Vector3 pos)
        {
            var v = pos - _forward;
            if (v.y > 0 && Vector3.Angle(_forward,v) <= _boidData.DetectionAngle)
            {
                return true;
            }

            return false;
        }
        /// <summary>
        /// 由于我们的碰撞体，在Root下面的一层，所以肯定要从父对象获取instanceID
        /// </summary>
        /// <param name="hit"></param>
        /// <returns></returns>
        int GetRootObjInstanceId(RaycastHit2D hit)
        {
            
            //带有刚体的会碰到刚体上，带有coll的会碰到coll上
            return hit.transform.gameObject.GetInstanceID();
        }
        

        #endregion

        #region BoidsSimulate
        //获取 List<Boid>当中 所有Boid 的平均位置
        public Vector3 GetAveragePosition(LinkedList<Boid> someBoids)
        {
            Vector3 sum = Vector3.zero;
            foreach (Boid b in someBoids)
                sum += b.transform.position;
            Vector3 center = sum / someBoids.Count;

            return (center);
        }
        
        //获取 List<Boid> 当中 所有Boid 的平均速度
        public Vector3 GetAverageVelocity(LinkedList<Boid> someBoids)
        {
            Vector3 sum = Vector3.zero;
            foreach (Boid b in someBoids)
                sum += b.Forward;
            Vector3 avg = sum / someBoids.Count;
            return (avg);
        }
        

        #endregion
        
    }

}
