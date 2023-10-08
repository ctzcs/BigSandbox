using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Optimization
{
    public class Node : IHeapItem<Node>
    {
        public bool walkable;
        public Vector3 worldPos;
        public int gridX;
        public int gridY;
        public int movementPenalty;//权重

        public int gCost;
        public int hCost;
        public int fCost { get { return gCost + hCost; } }

        public Node parent;

        private int heapIndex;
        public int HeapIndex { get { return heapIndex; } set { heapIndex = value; } }

        public Node(bool walkable, Vector3 worldPos, int gridX, int gridY, int movementPenalty)
        {
            this.walkable = walkable;
            this.worldPos = worldPos;
            this.gridX = gridX;
            this.gridY = gridY;
            this.movementPenalty = movementPenalty;
        }

        public int CompareTo(Node other) //比较器
        {
            int compare = fCost.CompareTo(other.fCost);
            if (compare == 0) 
            {
                compare = hCost.CompareTo(other.hCost);
            }
            return -compare;
        }
    }
}

