using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MoistureGen
{
    private class MoistureInfo
    {
        public Coord Coord;
        public Mercator mercator;
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

                float xPercent = (float)x / window.LonResolution;
                float yPercent = (float)y / window.LatResolution;

                float nMoisture = moistureNoise[merc.FlatSample] * Mathf.Max(world.MoistureIterations, 1);
                float nMax = world.MoistureData.Amplitude * Mathf.Max(world.MoistureIterations, 1);


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
        List<Coord> windOriginList = Wind.FindWindOriginBoundary(coord);
        List<MoistureInfo> moistureList = new List<MoistureInfo>();

        Mercator mercO = coord.ToMercator(window);
        foreach(Coord c in windOriginList)
        {
            Mercator m = c.ToMercator(window);
            MoistureInfo info = new MoistureInfo();
            info.Coord = c;
            info.mercator = m;

            float h1 = height[m.FlatSample];
            float h2 = mountain[m.FlatSample];

            info.Moisture = CalculateMoisture(h1, h2, world.OceanLevel);
            info.Amplitude = world.MoistureAmplitude;
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
