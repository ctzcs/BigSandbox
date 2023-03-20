using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace MyFlowField
{
    public class FieldRequestManager:MonoBehaviour
    {
        private static FieldRequestManager _instance;
        private FlowField _flowField = new FlowField();

        private Queue<PathResult> results = new Queue<PathResult>();

        private void Awake()
        {
            _instance = this.GetComponent<FieldRequestManager>();
        }

        private void Update()
        {
            if (results.Count > 0)
            {
                int count = results.Count;
                lock (results)
                {
                    for (int i = 0; i < count; i++)
                    {
                        PathResult result = results.Dequeue();
                        //这里就是请求的时候的callback
                        result.callback(result.FlowField,result.Success);
                    }
                }
            }
        }

        /// <summary>
        /// 外部请求时开启一个线程
        /// </summary>
        /// <param name="request"></param>
        public static void FlowFieldRequest(PathRequest request)
        {
            Task t = Task.Run(() =>
            {
                //这里可以使用静态方法，也可以通过实例去调用方法
                _instance._flowField.FindPath(request, _instance.FinishFindingPath);
                /*FlowField.FindPath(request, _instance.FinishFindingPath);*/
            });

            /*ThreadPool.QueueUserWorkItem((obj) =>
            {
                _instance._flowField.FindPath(request, _instance.FinishFindingPath);
                Debug.Log(Thread.CurrentThread.ManagedThreadId);
            });*/
           

        }
    
        /// <summary>
        /// 返回一个线程
        /// </summary>
        /// <param name="result"></param>
        public void FinishFindingPath(PathResult result)
        {
            lock (results)
            {
                results.Enqueue(result);
            }
        }
    }
    
    public struct PathRequest
    {
        public Vector3 Target;//开始格子的索引
        public Vector3 StartPosition;
        public Vector2Int GridSize;
        public float CellRadius;
        public Action<FlowField, bool> Callback;

        public PathRequest(Vector3 target, Vector3 startPosition,Vector2Int gridSize,float cellRadius,Action<FlowField,bool> callback)
        {
            Target = target;
            StartPosition = startPosition;
            GridSize = gridSize;
            CellRadius = cellRadius;
            Callback = callback;
        }

    }
    public struct PathResult
    {
        public FlowField FlowField;
        public bool Success;
        public Action<FlowField, bool> callback;

        public PathResult(FlowField path,bool success,Action<FlowField, bool> callback)
        {
            Success = success;
            this.FlowField = path;
            this.callback = callback;
        }
    }
    
    
}