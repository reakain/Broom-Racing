using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class ProceduralComplexLoop
{
    // Source: https://www.gamasutra.com/blogs/GustavoMaciel/20131229/207833/Generating_Procedural_Racetracks.php
    public static Vector2[] GeneratePoints(float xBound, float yBound, int pointCount)
    { 

    //int pointCount = Random.Range(10, 20); //we'll have a total of 10 to 20 points  
    Vector2[] points = new Vector2[pointCount * 2];
        List<Vertex> convexhullvert = new List<Vertex>();
        for (int i = 0; i < pointCount; ++i)
        {
            float x = Random.Range(0.0f, xBound) - (xBound/2f);  // we subtract 125 to keep the square centralized  
            float y = Random.Range(0.0f, yBound) - (yBound/2f);
            points[i] = new Vector2(x, y);

            convexhullvert.Add(new Vertex(points[i]));
        }
        List<Vertex> dataset = ConvexHull.GetConvexHull_XY(convexhullvert);

        Vector2[] dataSet = new Vector2[dataset.Count];
        for(int i = 0; i < dataset.Count; i++)
        {
            dataSet[i] = dataset[i].GetPos2D_XY();
        }
        

        int pushIterations = 3;
        for (int i = 0; i < pushIterations; ++i)
        {
            pushApart(dataSet);
        }

        Vector2[] rSet = new Vector2[dataSet.Length * 2];
        Vector2 disp = new Vector2();
        float difficulty = 1f; //the closer the value is to 0, the harder the track should be. Grows exponentially.  
        float maxDisp = 10f; // Again, this may change to fit your units.  
        for (int i = 0; i < dataSet.Length; ++i)
        {
            float dispLen = (float)Mathf.Pow(Random.Range(0.0f, 1.0f), difficulty) * maxDisp;
            disp.Set(0, 1);
            disp.Rotate(Random.Range(0.0f, 1.0f) * 360);
            disp.Scale(new Vector2(dispLen,dispLen));
            rSet[i * 2] = dataSet[i];
            rSet[i * 2 + 1] = new Vector2(dataSet[i].x, dataSet[i].y);
            rSet[i * 2 + 1] += (dataSet[(i + 1) % dataSet.Length]);
            rSet[i * 2 + 1] /= 2;
            rSet[i*2+1] += (disp);
            //Explaining: a mid point can be found with (dataSet[i]+dataSet[i+1])/2.  
            //Then we just add the displacement.  
        }
        dataSet = rSet;
        //push apart again, so we can stabilize the points distances.  
        for (int i = 0; i < pushIterations; ++i)
        {
            pushApart(dataSet);
        }

        for (int i = 0; i < 10; ++i)
        {
            fixAngles(dataSet);
            pushApart(dataSet);
        }

        return dataSet;// CatmullRomSpline.GetCatmullRomSpline(dataSet);
        /*
        for (float i = 0; i <= 1.0f;)
        {
            Vector2 p = CatmullRom.calculatePoint(dataSet, i);
            Vector2 deriv = CatmullRom.calculateDerivative(dataSet, i);
            float len = deriv.Length();
            i += step / len;
            deriv.divide(len);
            deriv.scale(thickness);
            deriv.set(-deriv.y, deriv.x);
            Vector2 v1 = new Vector2();
            v1.set(p).add(deriv);
            vertices.add(v1);
            Vector2 v2 = new Vector2();
            v2.set(p).sub(deriv);
            vertices.add(v2);

            if (i > 1.0f) i = 1.0f;
        }
        */
    }

    public static void pushApart(Vector2[] dataSet)
    {
        float dst = 15; //I found that 15 is a good value, though maybe, depending on your scale you'll need other value.  
        float dst2 = dst * dst;
        for (int i = 0; i < dataSet.Length; ++i)
        {
            for (int j = i + 1; j < dataSet.Length; ++j)
            {
                if (Mathf.Pow(Vector2.Distance(dataSet[i], dataSet[j]),2f)  < dst2)
                {
                    float hx = dataSet[j].x - dataSet[i].x;
                    float hy = dataSet[j].y - dataSet[i].y;
                    float hl = (float)Mathf.Sqrt(hx * hx + hy * hy);
                    hx /= hl;
                    hy /= hl;
                    float dif = dst - hl;
                    hx *= dif;
                    hy *= dif;
                    dataSet[j].x += hx;
                    dataSet[j].y += hy;
                    dataSet[i].x -= hx;
                    dataSet[i].y -= hy;
                }
            }
        }
    }

    public static void fixAngles(Vector2[] dataSet)
    {
        for (int i = 0; i < dataSet.Length; ++i)
        {
            int previous = (i - 1 < 0) ? dataSet.Length - 1 : i - 1;
            int next = (i + 1) % dataSet.Length;
            float px = dataSet[i].x - dataSet[previous].x;
            float py = dataSet[i].y - dataSet[previous].y;
            float pl = (float)Mathf.Sqrt(px * px + py * py);
            px /= pl;
            py /= pl;

            float nx = dataSet[i].x - dataSet[next].x;
            float ny = dataSet[i].y - dataSet[next].y;
            nx = -nx;
            ny = -ny;
            float nl = (float)Mathf.Sqrt(nx * nx + ny * ny);
            nx /= nl;
            ny /= nl;
            //I got a vector going to the next and to the previous points, normalised.  

            float a = (float)Mathf.Atan2(px * ny - py * nx, px * nx + py * ny); // perp dot product between the previous and next point. Google it you should learn about it!  

            if (Mathf.Abs(a * Mathf.Rad2Deg) <= 100) continue;

            float nA = 100 * Mathf.Sign(a) * Mathf.Deg2Rad;
            float diff = nA - a;
            float cos = (float)Mathf.Cos(diff);
            float sin = (float)Mathf.Sin(diff);
            float newX = nx * cos - ny * sin;
            float newY = nx * sin + ny * cos;
            newX *= nl;
            newY *= nl;
            dataSet[next].x = dataSet[i].x + newX;
            dataSet[next].y = dataSet[i].y + newY;
            //I got the difference between the current angle and 100degrees, and built a new vector that puts the next point at 100 degrees.  
        }
    }
}
