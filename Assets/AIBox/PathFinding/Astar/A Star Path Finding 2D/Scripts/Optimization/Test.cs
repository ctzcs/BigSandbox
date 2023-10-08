using NaughtyAttributes;
using Optimization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public Transform Start;

    public Transform End;

    public PathFinding pathFinding;

    [Button("Find")]
    void PathFinding() 
    {
        if (Start != null && End != null)
        {
            pathFinding.FindPath(new PathRequest(Start.position, End.position, null), null);
        }
    }
}
