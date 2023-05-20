using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MoistureGen
{
    private class MoistureInfo
    {
        public float Moisture;
        public float Amplitude;
    };

    public static float[] RefineMap(float[] moistureNoise, float[] height, float[] mountain, WorldData world, ViewData window)
    {
        float[] moistureMap = new float[window.LonResolution * window.LatResolution];

        for (int y = 0; y < window.LatResolution; y++)
        {
            for (int x = 0; x < window.LonResolution; x++)
            {
                Mercator merc = new Mercator(x,y, window.LonResolution);
                float alt = height[merc.FlatSample] + mountain[merc.FlatSample];
                if (alt < world.OceanLevel)
                {
                    moistureMap[merc.FlatSample] = 1.0f;
                    continue;
                }                   

                float nMoisture = moistureNoise[merc.FlatSample];
                float nMax = 1.0f;

                float wMoisture = 0.0f;
                float wMax = 0.0f;
                Coord coord = merc.ToCoord(window);
                List<MoistureInfo> windOrigins = FindWindOriginList( coord, height, mountain , world, window);
                foreach (MoistureInfo o in windOrigins)
                {
                    wMoisture += o.Moisture*o.Amplitude;
                    wMax += o.Amplitude;                        
                }
                
                float moisture = (nMoisture + wMoisture)/(nMax + wMax);
                

                moistureMap[x* window.LatResolution + y] = moisture;
            }
        }

        return moistureMap;
    }

    private static List<MoistureInfo> FindWindOriginList(Coord coord, float[] height, float[] mountain, WorldData world, ViewData window)
    {
        List<Coord> windOriginList = Wind.FindWindOriginBoundary(coord, world.WindVelocity);
        List<MoistureInfo> moistureList = new List<MoistureInfo>();

        Mercator mercO = coord.ToMercator(window);
        Mercator v0 = windOriginList[0].ToMercator(window);
        Mercator v1 = windOriginList[1].ToMercator(window);
        List<int> samples = mercO.RasterizeFlatSamples(v0, v1);
        foreach(int sample in samples)
        {
            MoistureInfo info = new MoistureInfo();
            float hVal = height[sample];
            float mVal = mountain[sample];
            //if we are within a mountain range, then this wind-moisture sample wont make it to the origin
            //if (mVal > world.MountainData.Amplitude / 2)
            //   break;

            info.Moisture = CalculateMoisture(hVal, mVal, world.OceanLevel);
            //TODO reduce amplitude the further away from mecO this sample is.
            info.Amplitude = 1.0f;
            moistureList.Add(info);
        }

        return moistureList;
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
