using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Simple
{
    public class Node : IComparable<Node>
    {
        public bool walkable;
        public Vector3 worldPos;
        public int gridX;
        public int gridY;

        public int gCost;
        public int hCost;
        public int fCost { get { return gCost + hCost; } }

        public Node parent;

        public Node(bool walkable, Vector3 worldPos, int gridX, int gridY)
        {
            this.walkable = walkable;
            this.worldPos = worldPos;
            this.gridX = gridX;
            this.gridY = gridY;
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
