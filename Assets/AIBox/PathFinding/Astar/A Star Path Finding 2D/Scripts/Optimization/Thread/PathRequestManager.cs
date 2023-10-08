using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

namespace Optimization
{
    public struct PathRequest
    {
        public Vector3 pathStart;
        public Vector3 pathEnd;
        public Action<Vector3[], bool> callBack;//返回路点以及是否找到路径

        public PathRequest(Vector3 start, Vector3 end, Action<Vector3[], bool> callBack)
        {
            pathStart = start;
            pathEnd = end;
            this.callBack = callBack;
        }
    }

    public struct PathResult
    {
        public Vector3[] path;
        public bool success;
        public Action<Vector3[], bool> callBack;//返回路点以及是否找到路径

        public PathResult(Vector3[] path, bool success, Action<Vector3[], bool> callBack)
        {
            this.path = path;
            this.success = success;
            this.callBack = callBack;
        }
    }


    public class PathRequestManager : MonoBehaviour //处理所有的寻路请求
    {
        Queue<PathResult> results = new Queue<PathResult>();

        static PathRequestManager instance;
        PathFinding pathFinding;

        private void Awake()
        {
            instance = this;
            pathFinding = GetComponent<PathFinding>();
        }

        private void Update()
        {
            if (results.Count > 0) 
            {
                int itemsInQueue = results.Count;
                lock (results) 
                {
                    for (int i = 0; i < itemsInQueue; i++) 
                    {
                        PathResult result = results.Dequeue();
                        result.callBack(result.path, result.success);
                    }
                }
            }
        }

        //使用多线程
        public static void RequestPath(PathRequest request) 
        {
            ThreadStart threadStart = delegate
            {
                instance.pathFinding.FindPath(request, instance.FinishFindingPath);
            };

            threadStart.Invoke();
        }

        public void FinishFindingPath(PathResult result) 
        {
            lock (results) 
            {
                results.Enqueue(result);
            }
            
        }

    }
}
