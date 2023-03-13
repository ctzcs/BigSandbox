using UnityEngine;

namespace MyFlowField
{
    public class Cell
    {
        public Vector3 worldPos;
        public Vector2Int gridIndex;
        public byte cost;//格子的花费
        public ushort bestCost;//最好的花费
        public GridDirection bestDirection;//最好的方向

        public Cell(Vector3 _worldPos, Vector2Int _gridIndex)
        {
            worldPos = _worldPos;
            gridIndex = _gridIndex;
            cost = 1;
            bestCost = ushort.MaxValue;
            bestDirection = GridDirection.None;
        }

        public void IncreaseCost(int amnt)
        {
            if (cost == byte.MaxValue) { return; }
            if (amnt + cost >= 255) { cost = byte.MaxValue; }
            else { cost += (byte)amnt; }
        }
    }
}

