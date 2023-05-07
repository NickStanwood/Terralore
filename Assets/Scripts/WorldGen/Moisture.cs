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

                float nMoisture = moistureNoise[x * window.LatResolution + y];
                float nAmp = world.MoistureData.Amplitude;


                float iMoisture = 0.0f;
                float iAmp = 0.0f;
                float windAmp = 1.0f;
                for (int i = 0; i < world.MoistureIterations; i++)
                {
                    //convert mercator to index
                    int xSample = (int)(xPercent * window.LonResolution);
                    int ySample = (int)(yPercent * window.LatResolution);
                    int noiseIndex = xSample*window.LatResolution + ySample;

                    //sample height & mountain at index
                    float h = height[noiseIndex];
                    float m = mountain[noiseIndex];

                    iMoisture += CalculateMoisture(h, m, world.OceanLevel) * windAmp * world.MoistureAmplitude;
                    iAmp += windAmp * world.MoistureAmplitude;

                    //get new coord based on wind
                    Vector2 coord = Coordinates.MercatorToCoord(xPercent, yPercent, window);
                    coord = Wind.FindOrigin(coord);
                    windAmp = Wind.Velocity(coord);

                    //convert new coord to mercator
                    Vector2 newMercator = Coordinates.CoordToMercator(coord.x, coord.y, window);
                    xPercent = newMercator.x;
                    yPercent = newMercator.y;
                }
                
                float moisture = (nMoisture + iMoisture)/(nAmp + iAmp);
                

                moistureMap[x* window.LatResolution + y] = moisture;
            }
        }

        return moistureMap;
    }

    public static float CalculateMoisture(float height, float mountain, float oceanLevel)
    {
        float alt = height + mountain;
        if (alt < oceanLevel)
            return 1.0f;

        return 1.0f - (alt - oceanLevel);
    }
}
