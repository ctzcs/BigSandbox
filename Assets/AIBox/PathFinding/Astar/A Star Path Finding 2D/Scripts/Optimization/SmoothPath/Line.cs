
using UnityEngine;

public struct Line
{
    const float verticalLineGradient = 1e5f;

    //L1 : y = ax + b
    float gradient; //斜率 a
    float y_intercept; //截距 b
    float gradientPerpendicular; //垂直于L1的斜率

    Vector2 pointOnLine_1;
    Vector2 pointOnLine_2;

    bool approachSide;

    public Line(Vector2 pointOnLine, Vector2 pointPerpendicularToLine)
    {
        float dx = pointOnLine.x - pointPerpendicularToLine.x;
        float dy = pointOnLine.y - pointPerpendicularToLine.y;

        if (dx == 0) //垂直于x轴是斜率是无穷--给你一个极大值verticalLineGradient = 1e5f;
        {
            gradientPerpendicular = verticalLineGradient;
        }
        else
        {
            gradientPerpendicular = dy / dx;
        }

        if (gradientPerpendicular == 0) //垂直于y轴 斜率为0
        {
            gradient = verticalLineGradient;
        }
        else
        {
            gradient = -1 / gradientPerpendicular; //L1⊥L2 -- K1 * K2 = -1;
        }

        y_intercept = pointOnLine.y - gradient * pointOnLine.x;
        pointOnLine_1 = pointOnLine;
        pointOnLine_2 = pointOnLine + new Vector2(1, gradient);

        approachSide = false;
        approachSide = GetSide(pointPerpendicularToLine);
    }


    bool GetSide(Vector2 p)
    {
        return (p.x - pointOnLine_1.x) * (pointOnLine_2.y - pointOnLine_1.y) > (p.y - pointOnLine_1.y) * (pointOnLine_2.x - pointOnLine_1.x);
    }

    public bool HasCrossedLine(Vector2 p) 
    {
        return GetSide(p) != approachSide;
    }

    public float DistanceFromPoint(Vector2 p) 
    {
        float yInterceptPerpendicular = p.y - gradientPerpendicular * p.x;
        float intersectX = (yInterceptPerpendicular - y_intercept) / (gradient - gradientPerpendicular);
        float intersectY = gradient * intersectX + y_intercept;

        return Vector2.Distance(p, new Vector2(intersectX, intersectY));
    }

    public void DrawWidthGizmos(float length) 
    {
        Vector3 lineDir = new Vector3(1, gradient, 0).normalized;
        Vector3 lineCenter = new Vector3(pointOnLine_1.x, pointOnLine_1.y, 0);
        Gizmos.DrawLine(lineCenter - lineDir * length / 2, lineCenter + lineDir * length / 2);
    }
}
