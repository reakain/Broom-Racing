﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Interpolation between points with a Catmull-Rom spline
public static class CatmullRomSpline
{
    public static Vector2[] GetCatmullRomSpline(Vector2[] dataSet)
    {
        List<Vector2> newPoints = new List<Vector2>();
        for (int i = 0; i < dataSet.Length; i++)
        {
            newPoints.AddRange(CalculatePoint(dataSet, i));
        }

        return newPoints.ToArray();
    }

    public static Vector2[] CalculatePoint(Vector2[] dataSet, int pos)
    {
        //The 4 points we need to form a spline between p1 and p2
        Vector3 p0 = dataSet[ClampListPos(dataSet.Length, pos - 1)];
        Vector3 p1 = dataSet[pos];
        Vector3 p2 = dataSet[ClampListPos(dataSet.Length,pos + 1)];
        Vector3 p3 = dataSet[ClampListPos(dataSet.Length,pos + 2)];

        //The start position of the line
        Vector3 lastPos = p1;

        //The spline's resolution
        //Make sure it's is adding up to 1, so 0.3 will give a gap, but 0.2 will work
        float resolution = 0.2f;

        //How many times should we loop?
        int loops = Mathf.FloorToInt(1f / resolution);
        Vector2[] pathSeg = new Vector2[loops+1];
        pathSeg[0] = new Vector2(p1.x,p1.y);

        for (int i = 1; i <= loops; i++)
        {
            //Which t position are we at?
            float t = i * resolution;

            //Find the coordinate between the end points with a Catmull-Rom spline
            Vector3 newPos = GetCatmullRomPosition(t, p0, p1, p2, p3);

            //Save this pos so we can draw the next line segment
            lastPos = newPos;
            pathSeg[i] = new Vector2(newPos.x, newPos.y);
        }
        return pathSeg;
    }

    //Clamp the list positions to allow looping
    public static int ClampListPos(int listLength,int pos)
    {
        if (pos < 0)
        {
            pos = listLength - 1;
        }

        if (pos > listLength)
        {
            pos = 1;
        }
        else if (pos > listLength - 1)
        {
            pos = 0;
        }

        return pos;
    }

    //Returns a position between 4 Vector3 with Catmull-Rom spline algorithm
    //http://www.iquilezles.org/www/articles/minispline/minispline.htm
    public static Vector3 GetCatmullRomPosition(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        //The coefficients of the cubic polynomial (except the 0.5f * which I added later for performance)
        Vector3 a = 2f * p1;
        Vector3 b = p2 - p0;
        Vector3 c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
        Vector3 d = -p0 + 3f * p1 - 3f * p2 + p3;

        //The cubic polynomial: a + b * t + c * t^2 + d * t^3
        Vector3 pos = 0.5f * (a + (b * t) + (c * t * t) + (d * t * t * t));

        return pos;
    }
}