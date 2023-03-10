using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    public static float[,] GenerateNoiseMap(NoiseData noise, ViewData window)
    {
        float max, min;
        return GenerateNoiseMap(noise, window, out min, out max);
    }

    public static float[,] GenerateNoiseMap(NoiseData noise, ViewData window, out float localMinNoise, out float localMaxNoise)
    {
        float[,] noiseMap = new float[window.ResolutionX, window.ResolutionY];

        double windowSampleFreqX = window.Width / window.ResolutionX;
        double windowSampleFreqY = window.Height / window.ResolutionY;

        float absoluteMaxNoise = GetMaxNoise(noise);

        localMinNoise = float.MaxValue;
        localMaxNoise = float.MinValue;

        PerlinSampler sampler = new PerlinSampler(noise);

        for (int y = 0; y < window.ResolutionY; y++)
        {
            for (int x = 0; x < window.ResolutionX; x++)
            {
                double sampleX = x * windowSampleFreqX + window.X;
                double sampleY = y * windowSampleFreqY + window.Y;
                float noiseVal = sampler.Sample(sampleX, sampleY);

                if(noiseVal < localMinNoise)
                    localMinNoise = noiseVal;

                if(noiseVal > localMaxNoise)
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
