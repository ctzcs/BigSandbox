using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simple
{
    public class Grid : MonoBehaviour
    {
        //Debug
        public bool IsDisplayGrid = true;
        public float displayerAlpha = 0.2f;

        //Grid
        public LayerMask obstacleLayer;
        public Vector2 gridWorldSize;
        public float nodeRadius;

        private Node[,] grid;
        private float nodeDiameter;//一个格的大小
        private int gridSizeX;
        private int gridSizeY;
        public int MaxSize { get { return gridSizeX * gridSizeY; } }

        private List<Node> path;

        void Awake()
        {
            if (grid == null) CreateGrid();
        }

        [Button("CreateGrid")]
        private void CreateGrid()
        {
            nodeDiameter = nodeRadius * 2;
            gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
            gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
            grid = new Node[gridSizeX, gridSizeY];
            Vector3 worldBottomLeft = transform.position - new Vector3(gridWorldSize.x / 2, gridWorldSize.y / 2, 0);
            for (int x = 0; x < gridSizeX; x++)
            {
                for (int y = 0; y < gridSizeY; y++)
                {
                    Vector3 worldPos = worldBottomLeft + new Vector3(x * nodeDiameter + nodeRadius, y * nodeDiameter + nodeRadius, 0);
                    bool walkable = !Physics2D.OverlapCircle(worldPos, nodeRadius, obstacleLayer);
                    grid[x, y] = new Node(walkable, worldPos, x, y);
                }
            }
        }

        public Node NodeFromWorldPos(Vector3 worldPos)
        {
            float xPercent = (worldPos.x + gridWorldSize.x / 2) / gridWorldSize.x;
            float yPercent = (worldPos.y + gridWorldSize.y / 2) / gridWorldSize.y;
            xPercent = Mathf.Clamp01(xPercent);
            yPercent = Mathf.Clamp01(yPercent);

            int x = Mathf.RoundToInt((gridSizeX - 1) * xPercent);
            int y = Mathf.RoundToInt((gridSizeY - 1) * yPercent);
            return grid[x, y];
        }

        public List<Node> GetNeighbours(Node node)
        {
            List<Node> neightbours = new List<Node>();
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0)
                        continue;
                    int x = node.gridX + i;
                    int y = node.gridY + j;
                    if (x >= 0 && x < gridSizeX && y >= 0 && y < gridSizeY)
                    {
                        neightbours.Add(grid[x, y]);
                    }
                }
            }

            return neightbours;
        }

        public void SetPath(List<Node> path)
        {
            this.path = path;
        }

        private void OnDrawGizmos()
        {
            if (IsDisplayGrid)
            {
                Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 0));
                if (grid != null) 
                {
                    foreach (Node node in grid)
                    {
                        Gizmos.color = node.walkable ? new Color(0, 1, 0, displayerAlpha) : new Color(1, 0, 0, displayerAlpha);
                        if (path != null && path.Contains(node))
                        {
                            Gizmos.color = Color.yellow;
                        }
                        Gizmos.DrawCube(node.worldPos, Vector3.one * (nodeDiameter - 0.1f));
                    }

                }

            }
        }
    }
}