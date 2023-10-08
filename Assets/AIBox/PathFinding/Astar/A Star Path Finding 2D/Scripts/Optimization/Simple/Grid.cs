using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Optimization
{
    [System.Serializable]
    public class TerrainType
    {
        public LayerMask terrainMask;
        public int terrainPenalty;
    }

    public class Grid : MonoBehaviour
    {
        //Debug
        public bool IsDisplayGrid = true;
        public float displayerAlpha = 0.2f;

        int penaltyMin = int.MaxValue;
        int penaltyMax = int.MinValue;

        public TerrainType[] Terrains;
        public LayerMask obstacleLayer;
        private LayerMask walkableLayer;
        private Dictionary<int, int> WalkableLayerWeight;

        public Vector2 gridWorldSize;
        public float nodeRadius;

        private Node[,] grid;
        private float nodeDiameter;//一个格的大小
        private int gridSizeX;
        private int gridSizeY;
        public int MaxSize { get { return gridSizeX * gridSizeY; } }

        private List<Node> path;

        public int blurSize = 3;
        public int obstacleProximityPenalty = 10;

        void Awake()
        {
            if (grid == null) GreateGrid();
        }

        private void Init() 
        {
            if (WalkableLayerWeight == null)
            {
                WalkableLayerWeight = new Dictionary<int, int>();
                foreach (TerrainType terrainType in Terrains)
                {
                    walkableLayer.value += terrainType.terrainMask.value;
                    WalkableLayerWeight.Add((int)Mathf.Log(terrainType.terrainMask.value, 2), terrainType.terrainPenalty);
                }
            }
        }

        [Button("CreateGrid")]
        private void GreateGrid()
        {
            Init();

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
                    //HACK 加入权重 -- 不同地形不同权重
                    int movementPenalty = 0;
                    RaycastHit2D hit2D = Physics2D.Raycast(worldPos, Vector2.right, nodeRadius * 0.5f, walkableLayer);
                    if (hit2D)
                    {
                        WalkableLayerWeight.TryGetValue(hit2D.collider.gameObject.layer, out movementPenalty);
                    }

                    if (!walkable)
                    {
                        movementPenalty += obstacleProximityPenalty;
                    }

                    grid[x, y] = new Node(walkable, worldPos, x, y, movementPenalty);
                }
            }

            BlurPenaltyMap(blurSize);
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

        //HACK 平滑网格路径
        //使用模糊核生成路面权重
        private void BlurPenaltyMap(int blurSize)
        {
            int kernalSize = blurSize * 2 + 1;
            int kernalExtents = (kernalSize - 1) / 2;

            int[,] penaltyHorizontalPass = new int[gridSizeX, gridSizeY];
            int[,] penaltyVerticalPass = new int[gridSizeX, gridSizeY];

            for (int y = 0; y < gridSizeY; y++)
            {
                for (int x = -kernalExtents; x <= kernalExtents; x++)
                {
                    int sampleX = Mathf.Clamp(x, 0, kernalExtents);
                    penaltyHorizontalPass[0, y] += grid[sampleX, y].movementPenalty;
                }

                for (int x = 1; x < gridSizeX; x++)
                {
                    int removeIndex = Mathf.Clamp(x - kernalExtents - 1, 0, gridSizeX);
                    int addIndex = Mathf.Clamp(x + kernalExtents, 0, gridSizeX - 1);

                    penaltyHorizontalPass[x, y] = penaltyHorizontalPass[x - 1, y] - grid[removeIndex, y].movementPenalty + grid[addIndex, y].movementPenalty;
                }
            }

            for (int x = 0; x < gridSizeX; x++)
            {
                for (int y = -kernalExtents; y <= kernalExtents; y++)
                {
                    int sampleY = Mathf.Clamp(y, 0, kernalExtents);
                    penaltyVerticalPass[x, 0] += penaltyHorizontalPass[x, sampleY];
                }

                int blurPenalty = Mathf.RoundToInt((float)penaltyVerticalPass[x, 0] / (kernalSize * kernalSize));
                grid[x, 0].movementPenalty = blurPenalty;

                for (int y = 1; y < gridSizeY; y++)
                {
                    int removeIndex = Mathf.Clamp(y - kernalExtents - 1, 0, gridSizeY);
                    int addIndex = Mathf.Clamp(y + kernalExtents, 0, gridSizeY - 1);

                    penaltyVerticalPass[x, y] = penaltyVerticalPass[x, y - 1] - penaltyHorizontalPass[x, removeIndex] + penaltyHorizontalPass[x, addIndex];
                    blurPenalty = Mathf.RoundToInt((float)penaltyVerticalPass[x, y] / (kernalSize * kernalSize));
                    grid[x, y].movementPenalty = blurPenalty;

                    if (blurPenalty > penaltyMax) 
                    {
                        penaltyMax = blurPenalty;
                    }

                    if (blurPenalty < penaltyMin) 
                    {
                        penaltyMin = blurPenalty;
                    }
                }
            }
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
                        Gizmos.color = Color.Lerp(new Color(1, 1, 1, displayerAlpha), new Color(0, 0, 0, displayerAlpha), Mathf.InverseLerp(penaltyMin, penaltyMax, node.movementPenalty));
                        Gizmos.color = node.walkable ? Gizmos.color : Color.red;
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
