using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Simple
{
    public class PathFinding : MonoBehaviour
    {
        private Grid grid;

        public Transform Start;

        public Transform End;

        void Awake()
        { 
            grid = GetComponent<Grid>();
        }

        [Button("Find")]
        private void Test()
        {
            grid = GetComponent<Grid>();

            if (Start != null && End != null) 
            {
                FindPath(Start.position, End.position);
            }

        }

        private void FindPath(Vector3 startPos, Vector3 endPos)
        {
            //加一个时间来测试我们的性能
            Stopwatch sw = new Stopwatch();
            sw.Start();

            Vector3[] wayPoints = new Vector3[0];
            bool pathFound = false;

            Node startNode = grid.NodeFromWorldPos(startPos);
            Node targetNode = grid.NodeFromWorldPos(endPos);

            if (startNode.walkable && targetNode.walkable)
            {
                List<Node> openList = new List<Node>();
                HashSet<Node> closeSet = new HashSet<Node>();

                openList.Add(startNode);

                while (openList.Count > 0)
                {
                    Node currentNode = openList[0];
                    for (int i = 1; i < openList.Count; i++)
                    {
                        if (openList[i].fCost < currentNode.fCost || openList[i].fCost == currentNode.fCost && openList[i].hCost < currentNode.hCost)
                        {
                            currentNode = openList[i];
                        }
                    }
                    openList.Remove(currentNode);
                    closeSet.Add(currentNode);

                    if (currentNode == targetNode)
                    {
                        sw.Stop();
                        print("Path found :" + sw.ElapsedMilliseconds + "ms");
                        pathFound = true;
                        break;
                    }

                    foreach (Node neightbour in grid.GetNeighbours(currentNode))
                    {
                        if (!neightbour.walkable || closeSet.Contains(neightbour))
                        {
                            continue;
                        }

                        int newMovementCostToNeightbour = currentNode.gCost + GetDistance(currentNode, neightbour);
                        if (newMovementCostToNeightbour < neightbour.gCost || !openList.Contains(neightbour))
                        {
                            neightbour.gCost = newMovementCostToNeightbour;
                            neightbour.hCost = GetDistance(neightbour, targetNode);
                            neightbour.parent = currentNode;

                            if (!openList.Contains(neightbour))
                            {
                                openList.Add(neightbour);
                            }
                        }
                    }
                }
            }

            if (pathFound)
            {
                wayPoints = RetracePath(startNode, targetNode);
            }
            else 
            {
                print("Path can not found...");
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
            grid.SetPath(path);
            return SimplifyPath(path);
        }

        private Vector3[] SimplifyPath(List<Node> path) 
        {
            List<Vector3> wayPoints = new List<Vector3>();
            Vector2 directionOld = Vector2.zero;

            for (int i = 1; i < path.Count; i++) 
            {
                Vector2 directionToNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
                if (directionToNew != directionOld) 
                {
                    wayPoints.Add(path[i].worldPos);
                }
                directionOld = directionToNew;
            }

            return wayPoints.ToArray();
        }

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
