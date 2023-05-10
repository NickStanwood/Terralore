using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MoistureGen
{
    public static float[] RefineMap(float[] moistureNoise, float[] height, float[] mountain, WorldData world, ViewData window)
    {
        float[] moistureMap = new float[window.LonResolution * window.LatResolution];

        for (int y = 0; y < window.LatResolution; y++)
        {
            for (int x = 0; x < window.LonResolution; x++)
            {
                float alt = height[x* window.LatResolution + y] + mountain[x * window.LatResolution + y];
                if (alt < world.OceanLevel)
                {
                    moistureMap[x * window.LatResolution + y] = 1.0f;
                    continue;
                }                   

                float xPercent = (float)x / window.LonResolution;
                float yPercent = (float)y / window.LatResolution;

                float nMoisture = moistureNoise[x * window.LatResolution + y] * Mathf.Max(world.MoistureIterations, 1);
                float nMax = world.MoistureData.Amplitude * Mathf.Max(world.MoistureIterations, 1);


                float wMoisture = 0.0f;
                float wMax = 0.0f;
                float windAmp = 1.0f;
                List<Vector2> windOrigins = FindWindOriginList( Coordinates.MercatorToCoord(xPercent, yPercent, window) , world.MoistureIterations);
                foreach (Vector2 o in windOrigins)
                {
                    windAmp = Wind.Velocity(o);

                    //convert new coord to mercator
                    Vector2 mercator = Coordinates.CoordToMercator(o.x, o.y, window);

                    //convert mercator to index
                    int xSample = (int)(mercator.x * window.LonResolution);
                    int ySample = (int)(mercator.y * window.LatResolution);
                    int noiseIndex = xSample * window.LatResolution + ySample;

                    //sample height & mountain at index
                    float h = height[noiseIndex];
                    float m = mountain[noiseIndex];

                    wMoisture += CalculateMoisture(h, m, world.OceanLevel) * windAmp * world.MoistureAmplitude;
                    wMax += windAmp * world.MoistureAmplitude;

                        
                }
                
                float moisture = (nMoisture + wMoisture)/(nMax + wMax);
                

                moistureMap[x* window.LatResolution + y] = moisture;
            }
        }

        return moistureMap;
    }

    private static List<Vector2> FindWindOriginList(Vector2 coord, int iterations)
    {
        List<Vector2> windOriginList = new List<Vector2>();

        Vector2 c0 = coord;
        Vector2 c1 = new Vector2(coord.x, coord.y + Mathf.PI / 60);
        Vector2 c2 = new Vector2(coord.x, coord.y - Mathf.PI / 60);
        for (int i = 0; i < iterations; i++)
        {
            windOriginList.Add(c0);
            //windOriginList.Add(c1);
            //windOriginList.Add(c2);

            c0 = Wind.FindOrigin(c0);
            //c1 = Wind.FindOrigin(c1);
            //c2 = Wind.FindOrigin(c2);

        }
        return windOriginList;
    }

    public static float CalculateMoisture(float height, float mountain, float oceanLevel)
    {
        float alt = height + mountain;
        if (alt < oceanLevel)
            return 1.0f;

        if (alt > 1.0f)
            return 0.0f;

        return (1.0f - oceanLevel)*(1.0f - alt);
    }
}
