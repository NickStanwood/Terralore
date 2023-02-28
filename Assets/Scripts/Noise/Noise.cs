using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    public static float[,] GenerateNoiseMap(int seed, NoiseParams noise, ViewWindow window)
    {
        float[,] noiseMap = new float[window.ResolutionX, window.ResolutionY];

        int[] seedOffsets = GetOctaveOffsets(seed, noise.Octaves);

        float windowSampleFreqX = window.Width / window.ResolutionX;
        float windowSampleFreqY = window.Height / window.ResolutionY;

        float maxNoise = GetMaxNoise(noise);

        for (int y = 0; y < window.ResolutionY; y++)
        {
            for (int x = 0; x < window.ResolutionX; x++)
            {
                float amp = 1.0f;
                float noiseVal = 0.0f;
                float freq = 1.0f;

                for (int i = 0; i < noise.Octaves; i++)
                {
                    float sampleX = (x + window.X + seedOffsets[i]) * windowSampleFreqX * noise.Frequency * freq;
                    float sampleY = (y + window.Y + seedOffsets[i]) * windowSampleFreqY * noise.Frequency * freq;
                    float perlin = Mathf.PerlinNoise(sampleX, sampleY);
                    //Debug.Log($"perlin [{x},{y}]: [{sampleX},{sampleY}] = {perlin}\nScale={scale}");
                    noiseVal += perlin * amp;

                    amp *= noise.Persistence;
                    freq *= noise.Lacunarity;
                }

                noiseMap[x, y] = noiseVal;
            }
        }
        Debug.Log($"max noise: {maxNoise}");
        return Normalize(noiseMap, maxNoise, 0.0f);
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

        return Normalize(noiseMap, max, min);
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

    public static int[] GetOctaveOffsets(int seed, int octaves)
    {
        System.Random rand = new System.Random(seed);
        int[] seedOffsets = new int[octaves];
        for (int i = 0; i < octaves; i++)
        {
            if (seed == 0)
                seedOffsets[i] = 0;
            else
                seedOffsets[i] = rand.Next(-100000, 100000);
        }

        return seedOffsets;
    }

    public static float GetMaxNoise(NoiseParams noise)
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
