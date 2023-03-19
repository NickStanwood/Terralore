using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    public static float[,] GenerateNoiseMap(NoiseData noise, ViewData window, TerrainData terrain)
    {
        float max, min;
        return GenerateNoiseMap(noise, window, terrain, out min, out max);
    }

    public static float[,] GenerateNoiseMap(NoiseData noise, ViewData window, TerrainData terrain, out float localMinNoise, out float localMaxNoise)
    {
        float[,] noiseMap = new float[window.LonResolution, window.LatResolution];

        double lonSampleFreq = window.LonAngle / window.LonResolution;
        double latSampleFreq = window.LatAngle / window.LatResolution;

        float absoluteMaxNoise = GetMaxNoise(noise);

        localMinNoise = float.MaxValue;
        localMaxNoise = float.MinValue;

        PerlinSampler sampler = new PerlinSampler(noise);

        for (int y = 0; y < window.LatResolution; y++)
        {
            for (int x = 0; x < window.LonResolution; x++)
            {
                Vector3 c = Coordinates.MercatorToCartesian(x, y, window, (float)terrain.WorldRadius);

                //Debug.Log($"(x, y, z)-sphere ({xSphere}, {ySphere}, {zSphere})");
                float noiseVal = sampler.Sample(c.x, c.y, c.z);

                if (noiseVal < localMinNoise)
                    localMinNoise = noiseVal;

                if (noiseVal > localMaxNoise)
                    localMaxNoise = noiseVal;

                noiseMap[x, y] = noiseVal;
            }
        }
        Debug.Log($"max noise: {absoluteMaxNoise}");

        //normalize max and min vlaues as well as noisemap
        localMaxNoise = localMaxNoise / absoluteMaxNoise;
        localMinNoise = localMinNoise / absoluteMaxNoise;
        return Normalize(noiseMap, absoluteMaxNoise, 0.0f);
    }

    public static float[,] Combine(List<float[,]> noiseLayers, int width, int height)
    {
        float max = 0.0f;
        float min = float.MaxValue;

        float[,] noiseMap = new float[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float noiseVal = 0.0f;
                foreach (float[,] layer in noiseLayers)
                {
                    noiseVal += layer[x, y];
                }

                if (noiseVal > max)
                    max = noiseVal;

                if (noiseVal < min)
                    min = noiseVal;

                noiseMap[x, y] = noiseVal;
            }
        }

        return noiseMap;
    }

    public static float[,] Normalize(float[,] noiseMap, float max, float min)
    {
        if (max == min)
            return noiseMap;

        for (int x = 0; x < noiseMap.GetLength(0); x++)
        {
            for (int y = 0; y < noiseMap.GetLength(1); y++)
            {
                noiseMap[x, y] = (noiseMap[x, y] - min) / (max - min);
            }
        }

        return noiseMap;

    }

    public static float[] GetOctaveOffsets(int seed, int octaves)
    {
        System.Random rand = new System.Random(seed);
        float[] seedOffsets = new float[octaves];
        for (int i = 0; i < octaves; i++)
        {
            seedOffsets[i] = rand.Next(-1000, 1000);
        }

        return seedOffsets;
    }

    public static float GetMaxNoise(NoiseData noise)
    {
        float maxNoise = 0.0f;
        float amp = 1.0f;
        for (int i = 0; i < noise.Octaves; i++)
        {
            maxNoise += amp;
            amp *= noise.Persistence;
        }
        return maxNoise;
    }

}
