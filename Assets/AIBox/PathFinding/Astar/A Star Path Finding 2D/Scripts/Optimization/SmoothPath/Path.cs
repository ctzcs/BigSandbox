using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path 
{
    public readonly Vector3[] lookPoints;
    public readonly Line[] turnBoundaries;
    public readonly int finishLineIndex;
    public readonly int slowDownIndex;

    public Path(Vector3[] wayPoints, Vector3 startPos, float turnDst, float stopDistance) 
    {
        lookPoints = wayPoints;
        turnBoundaries = new Line[lookPoints.Length];
        finishLineIndex = turnBoundaries.Length - 1;

        Vector2 previousPoint = startPos;
        for (int i = 0; i < lookPoints.Length; i++)
        {
            Vector2 currentPoint = lookPoints[i];
            Vector2 directionToCurrentPoint = (currentPoint - previousPoint).normalized;
            Vector2 turnBoundaryPoint = (i == finishLineIndex) ? currentPoint : currentPoint - directionToCurrentPoint * turnDst;
            turnBoundaries[i] = new Line(turnBoundaryPoint, previousPoint - directionToCurrentPoint * turnDst);
            previousPoint = turnBoundaryPoint;
        }

        float distanceFromPoint = 0;
        for (int i = lookPoints.Length - 1; i > 0; i--) 
        {
            distanceFromPoint += Vector3.Distance(lookPoints[i], lookPoints[i - 1]);
            if (distanceFromPoint > stopDistance) 
            {
                slowDownIndex = i;
                break;
            }
        }
    }

    public void DrawWithGizmos() 
    {
        Gizmos.color = Color.black;
        foreach (Vector3 p in lookPoints) 
        {
            Gizmos.DrawSphere(p, 0.2f);
        }

        Gizmos.color = Color.white;
        foreach (Line l in turnBoundaries) 
        {
            l.DrawWidthGizmos(1);
        }
    }
}
