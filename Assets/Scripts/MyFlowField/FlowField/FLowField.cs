using System;
using System.Collections.Generic;
using UnityEngine;

namespace MyFlowField
{
    public class FlowField
	{
		//所有的格子
		public Cell[,] grids { get; private set; }
		//格子的横纵数量
		public Vector2Int gridSize { get; private set; }
		//格子的半径
		public float cellRadius { get; private set; }
		//最终寻路所需目标格子
		public Cell destinationCell;
		//格子的直径
		private float m_cellDiameter;
		//创建格子的起点
		private Vector3 m_startPoint;

		//检查队列
		private Queue<Cell> _cellsToCheck;

		/// <summary>
		///获取邻居的容器，因为每帧调用这里为了节省内存
		/// </summary>
		private List<Cell> _neighborCells;

		public FlowField()
		{
			
		}
		public FlowField(float _cellRadius, Vector2Int _gridSize,Vector3 _startPoint)
		{
			cellRadius = _cellRadius;
			m_cellDiameter = cellRadius * 2f;
			gridSize = _gridSize;
			m_startPoint = _startPoint;
			_cellsToCheck = new Queue<Cell>(16);
			_neighborCells = new List<Cell>(16);
		}
		
		
		/// <summary>
		/// 根据世界坐标，创建格子
		/// </summary>
		public void CreateGrid()
		{
			grids = new Cell[gridSize.x, gridSize.y];
			for (int x = 0; x < gridSize.x; x++)
			{
				for (int y = 0; y < gridSize.y; y++)
				{
					//修改世界坐标grid
					Vector3 worldPos = m_startPoint + new Vector3(m_cellDiameter * x + cellRadius, m_cellDiameter * y + cellRadius,0 ) ;
					grids[x, y] = new Cell(worldPos, new Vector2Int(x, y));
				}
			}
		}
		
		/// <summary>
		/// 根据场景上的碰撞,创建场的花费
		/// </summary>
		public void CreateCostField()
		{
			Vector3 cellHalfExtents = Vector3.one * cellRadius;
			int terrainMask = LayerMask.GetMask("Impassible", "RoughTerrain");
			//这里暂时设置最多有是个障碍物
			Collider2D[] obstacles = new Collider2D[10];
			//这就是遍历每个Cell，如果cell中有障碍物，就将这个cell设置成255
			//如果是沙地设置成3
			foreach (Cell curCell in grids)
			{
				int count = Physics2D.OverlapBoxNonAlloc(m_startPoint+ curCell.worldPos, cellHalfExtents,0, obstacles,terrainMask);
				bool hasIncreasedCost = false;
				for (int i = 0; i < count; i++)
				{
					if (obstacles[i].gameObject.layer == 8)
					{
						curCell.IncreaseCost(255);
						continue;
					}
					if (!hasIncreasedCost && obstacles[i].gameObject.layer == 9)
					{
						curCell.IncreaseCost(3);
						hasIncreasedCost = true;
					}
				}
			}
		}
		/// <summary>
		/// 设置一个目标点，然后改变周围的花费
		/// </summary>
		/// <param name="_destinationCell"></param>
		public void CreateIntegrationField(Cell _destinationCell)
		{
			if (destinationCell != null 
				&& destinationCell != _destinationCell)
			{
				destinationCell.cost = 1;
				destinationCell.bestCost = ushort.MaxValue;
				//将所有的格子里的最小花费重置。
				foreach (var cell in grids)
				{
					cell.bestCost = ushort.MaxValue;
				}
			}
			destinationCell = _destinationCell;

			destinationCell.cost = 0;
			destinationCell.bestCost = 0;
			
			//创建一个需要检查的队列
			
			//将目标入队
			_cellsToCheck.Enqueue(destinationCell);
			//只要队列中还有元素，一直检查
			while(_cellsToCheck.Count > 0)
			{
				//出队，获取Cell的邻居网格
				Cell curCell = _cellsToCheck.Dequeue();
				List<Cell> curNeighbors = GetNeighborCells(curCell.gridIndex, GridDirection.CardinalDirections);
				//如果邻居花费和当前细胞花费的总和小于邻居的最小消耗，那么修改最小消耗，将邻居入队
				foreach (Cell curNeighbor in curNeighbors)
				{
					if (curNeighbor.cost == byte.MaxValue) { continue; }
					if (curNeighbor.cost + curCell.bestCost < curNeighbor.bestCost)
					{
						curNeighbor.bestCost = (ushort)(curNeighbor.cost + curCell.bestCost);
						_cellsToCheck.Enqueue(curNeighbor);
					}
				}
			}
		}
		/// <summary>
		/// 根据最小消耗创建流场的方向
		/// </summary>
		public void CreateFlowField()
		{
			foreach(Cell curCell in grids)
			{
				List<Cell> curNeighbors = GetNeighborCells(curCell.gridIndex, GridDirection.AllDirections);

				int bestCost = curCell.bestCost;

				foreach(Cell curNeighbor in curNeighbors)
				{
					if(curNeighbor.bestCost < bestCost)
					{
						bestCost = curNeighbor.bestCost;
						curCell.bestDirection = GridDirection.GetDirectionFromV2I(curNeighbor.gridIndex - curCell.gridIndex);
					}
				}
			}
		}
		
		/// <summary>
		/// 获取邻居格子
		/// </summary>
		/// <param name="nodeIndex"></param>
		/// <param name="directions"></param>
		/// <returns></returns>
		private List<Cell> GetNeighborCells(Vector2Int nodeIndex, List<GridDirection> directions)
		{
			//每次清空容器
			_neighborCells.Clear();
			foreach (Vector2Int curDirection in directions)
			{
				Cell newNeighbor = GetCellAtRelativePos(nodeIndex, curDirection);
				if (newNeighbor != null)
				{
					_neighborCells.Add(newNeighbor);
				}
			}
			return _neighborCells;
		}
		
		/// <summary>
		/// 通过索引获取相关位置的格子
		/// </summary>
		/// <param name="orignPos"></param>
		/// <param name="relativePos"></param>
		/// <returns></returns>
		private Cell GetCellAtRelativePos(Vector2Int orignPos, Vector2Int relativePos)
		{
			Vector2Int finalPos = orignPos + relativePos;

			if (finalPos.x < 0 || finalPos.x >= gridSize.x || finalPos.y < 0 || finalPos.y >= gridSize.y)
			{
				return null;
			}

			else { return grids[finalPos.x, finalPos.y]; }
		}
		
		/// <summary>
		/// 从世界坐标获取格子
		/// </summary>
		/// <param name="worldPos"></param>
		/// <returns></returns>
		public Cell GetCellFromWorldPos(Vector3 worldPos)
		{
			var pos = worldPos + m_startPoint; 
			float percentX = pos.x  / (gridSize.x * m_cellDiameter);
			float percentY = pos.y / (gridSize.y * m_cellDiameter);

			percentX = Mathf.Clamp01(percentX);
			percentY = Mathf.Clamp01(percentY);

			int x = Mathf.Clamp(Mathf.FloorToInt((gridSize.x) * percentX), 0, gridSize.x - 1);
			int y = Mathf.Clamp(Mathf.FloorToInt((gridSize.y) * percentY), 0, gridSize.y - 1);
			return grids[x, y];
		}

		/// <summary>
		/// 寻路方法
		/// </summary>
		/// <param name="request"></param>
		/// <param name="callback"></param>
		public void FindPath(PathRequest request,Action<PathResult> callback)
		{
			FlowField curFlowField;
			curFlowField = new FlowField(request.CellRadius, request.GridSize,request.StartPosition);
			curFlowField.CreateGrid();
			Cell destinationCell = curFlowField.GetCellFromWorldPos(request.Target);
			curFlowField.CreateIntegrationField(destinationCell);
			curFlowField.CreateFlowField();
			PathResult result = new PathResult(curFlowField,true,request.Callback);
			if (callback != null)
			{
				callback(result);
			}
			
		}
	}

}