using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector2Extension
{
    public static Vector2 Rotate(this Vector2 v, float degrees)
    {
        float radians = degrees * Mathf.Deg2Rad;
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);

        float tx = v.x;
        float ty = v.y;

        return new Vector2(cos * tx - sin * ty, sin * tx + cos * ty);
    }

    public static Vector2 RotateTowards(this Vector2 v, Vector2 other)
    {
        float radians = AngleBetween(v, other);
        return Rotate(v, radians * Mathf.Rad2Deg);
    }

    public static float AngleBetween(this Vector2 v, Vector2 otherV)
    {
        float radians = Mathf.Atan2(otherV.y - v.y, otherV.x-v.x);

        return radians;
    }

    public static float AngleBetweenDeg(this Vector2 v, Vector2 otherV)
    {
        return AngleBetween(v, otherV) * Mathf.Rad2Deg;
    }

    public static bool Intersects(Vector2 line1V1, Vector2 line1V2, Vector2 line2V1, Vector2 line2V2)//, out Vector2 intersectPoint)
    {
        //Line1
        float A1 = line1V2.y - line1V1.y;
        float B1 = line1V1.x - line1V2.x;
        float C1 = A1 * line1V1.x + B1 * line1V1.y;

        //Line2
        float A2 = line2V2.y - line2V1.y;
        float B2 = line2V1.x - line2V2.x;
        float C2 = A2 * line2V1.x + B2 * line2V1.y;

        float delta = A1 * B2 - A2 * B1;
        Vector2 intersectPoint = new Vector2(0, 0);

        if (delta == 0)
            return false;

        float x = (B2 * C1 - B1 * C2) / delta;
        float y = (A1 * C2 - A2 * C1) / delta;
        intersectPoint = new Vector2(x, y);

        float lineDist = Mathf.Abs(Vector2.Distance(line1V1, line1V2));

        if (Mathf.Abs(Vector2.Distance(intersectPoint, line1V1)) < lineDist && Mathf.Abs(Vector2.Distance(intersectPoint, line1V2)) < lineDist)
        {
            return true;
        }
        return false;

    }
}