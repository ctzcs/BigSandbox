using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Optimization
{
    public class PathFinding : MonoBehaviour
    {
        public Grid heapGrid;

        public void FindPath(PathRequest request, Action<PathResult> callback)
        {

            //加一个时间来测试我们的性能
            Stopwatch sw = new Stopwatch();
            sw.Start();

            Vector3[] wayPoints = new Vector3[0];
            bool pathFound = false;

            Node startNode = heapGrid.NodeFromWorldPos(request.pathStart);
            Node targetNode = heapGrid.NodeFromWorldPos(request.pathEnd);
            startNode.parent = startNode;

            //改善性能第一步:使用我们自己写的最小堆
            Heap<Node> openHeap = new Heap<Node>(heapGrid.MaxSize);
            HashSet<Node> closeSet = new HashSet<Node>();

            openHeap.Add(startNode);

            while (openHeap.Count > 0)
            {
                Node currentNode = openHeap.RemoveFirst();
                closeSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    sw.Stop();
                    print("Path found :" + sw.ElapsedMilliseconds + "ms");
                    pathFound = true;
                    break;
                }

                //HACK 使用二叉堆来存储
                foreach (Node neightbour in heapGrid.GetNeighbours(currentNode))
                {
                    if (!neightbour.walkable || closeSet.Contains(neightbour))
                    {
                        continue;
                    }

                    int newMovementCostToNeightbour = currentNode.gCost + GetDistance(currentNode, neightbour) + neightbour.movementPenalty;
                    if (newMovementCostToNeightbour < neightbour.gCost || !openHeap.Contains(neightbour))
                    {
                        neightbour.gCost = newMovementCostToNeightbour;
                        neightbour.hCost = GetDistance(neightbour, targetNode);
                        neightbour.parent = currentNode;

                        if (!openHeap.Contains(neightbour))
                        {
                            openHeap.Add(neightbour);
                        }
                        else 
                        {
                            openHeap.UpdateItem(neightbour);
                        }
                    }
                }
            }

            if (pathFound)
            {
                wayPoints = RetracePath(startNode, targetNode);
                pathFound = wayPoints.Length > 0;
            }

            if (callback != null) 
            {
                callback(new PathResult(wayPoints, pathFound, request.callBack));
            }
            
        }

        private Vector3[] RetracePath(Node start, Node end) 
        {
            List<Node> path = new List<Node>();
            Node currentNode = end;

            while (currentNode != start) 
            {
                path.Add(currentNode);
                currentNode = currentNode.parent;
            }

            path.Reverse();
            heapGrid.SetPath(path);
            return SimplifyPath(path);
        }

        Vector3[] SimplifyPath(List<Node> path)
        {
            List<Vector3> wayPoints = new List<Vector3>();
            Vector2 directionOld = Vector2.zero;

            for (int i = 1; i < path.Count; i++)
            {
                //HACK 路径优化（合并位于"同一水平线"上的点）
                Vector2 directionToNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
                if (directionToNew != directionOld)
                {
                    wayPoints.Add(path[i].worldPos);
                }
                directionOld = directionToNew;
            }

            wayPoints.Add(path[path.Count - 1].worldPos);

            return wayPoints.ToArray();
        }

        //HACK 距离使用整形计算
        private int GetDistance(Node A, Node B)
        {
            int disX = Mathf.Abs(A.gridX - B.gridX);
            int disY = Mathf.Abs(A.gridY - B.gridY);

            if (disX > disY)
                return disY * 14 + 10 * (disX - disY);
            return disX * 14 + 10 * (disY - disX);
        }
    }
}
