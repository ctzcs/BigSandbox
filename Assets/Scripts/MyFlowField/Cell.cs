using UnityEngine;

namespace MyFlowField
{
    public class Cell
    {
        public Vector3 worldPos;//世界坐标位置
        public Vector2Int gridIndex;//格子索引
        public byte cost;//本格消耗
        public ushort bestCost;//最低消耗
        public GridDirection bestDirection;//最低消耗方向

        public Cell(Vector3 _worldPos, Vector2Int _gridIndex)
        {
            worldPos = _worldPos;
            gridIndex = _gridIndex;
            cost = 1;
            bestCost = ushort.MaxValue;
            bestDirection = GridDirection.None;
        }
        
        /// <summary>
        /// 增加消耗
        /// </summary>
        /// <param name="amnt"></param>
        public void IncreaseCost(int amnt)
        {
            if (cost == byte.MaxValue) { return; }
            if (amnt + cost >= 255) { cost = byte.MaxValue; }
            else { cost += (byte)amnt; }
        }
    }
}

