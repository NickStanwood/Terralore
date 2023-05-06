using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MoistureGen
{
    public static float[] RefineMap(float[] moistureNoise, float[] height, float[] mountain, ViewData window, float worldRadius, int iterations)
    {
        float[] moistureMap = new float[window.LonResolution * window.LatResolution];

        for (int y = 0; y < window.LatResolution; y++)
        {
            for (int x = 0; x < window.LonResolution; x++)
            {
                float xPercent = (float)x / window.LonResolution;
                float yPercent = (float)y / window.LatResolution;

                float moisture = moistureNoise[x * window.LatResolution + y];
                float max = 1.0f;
                float windAmp = 1.0f;
                for (int i = 0; i < iterations; i++)
                {
                    //convert mercator to index
                    int xSample = (int)(xPercent * window.LonResolution);
                    int ySample = (int)(yPercent * window.LatResolution);
                    int noiseIndex = xSample*window.LatResolution + ySample;

                    //sample height & mountain at index
                    float h = height[noiseIndex];
                    float m = mountain[noiseIndex];

                    //TODO: add value to moisture

                    //get new coord based on wind
                    Vector2 coord = Coordinates.MercatorToCoord(xPercent, yPercent, window);
                    coord = Wind.FindOrigin(coord);
                    windAmp = Wind.Velocity(coord);

                    //convert new coord to mercator
                    Vector2 newMercator = Coordinates.CoordToMercator(coord.x, coord.y, window);
                    xPercent = newMercator.x;
                    yPercent = newMercator.y;
                }
                moisture /= max;
                

                moistureMap[x* window.LatResolution + y] = moisture;
            }
        }

        return moistureMap;
    }
}
