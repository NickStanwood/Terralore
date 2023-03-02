using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    public static float[,] GenerateNoiseMap(int seed, NoiseParams noise, ViewWindow window)
    {
        float max, min;
        return GenerateNoiseMap(seed, noise, window, out min, out max);
    }

    public static float[,] GenerateNoiseMap(int seed, NoiseParams noise, ViewWindow window, out float localMinNoise, out float localMaxNoise)
    {
        float[,] noiseMap = new float[window.ResolutionX, window.ResolutionY];

        int[] seedOffsets = GetOctaveOffsets(seed, noise.Octaves);

        float windowSampleFreqX = window.Width / window.ResolutionX;
        float windowSampleFreqY = window.Height / window.ResolutionY;

        float absoluteMaxNoise = GetMaxNoise(noise);

        localMinNoise = float.MaxValue;
        localMaxNoise = float.MinValue;

        for (int y = 0; y < window.ResolutionY; y++)
        {
            for (int x = 0; x < window.ResolutionX; x++)
            {
                float amp = 1.0f;
                float noiseVal = 0.0f;
                float freq = 1.0f;

                for (int i = 0; i < noise.Octaves; i++)
                {
                    float offsetX = seedOffsets[i] + window.X * noise.Frequency * freq;
                    float sampleX = x * windowSampleFreqX * noise.Frequency * freq + offsetX;


                    float offsetY = seedOffsets[i] + window.Y * noise.Frequency * freq;
                    float sampleY = y * windowSampleFreqY * noise.Frequency * freq + offsetY;
                    float perlin = Mathf.PerlinNoise(sampleX, sampleY);
                    //Debug.Log($"perlin [{x},{y}]: [{sampleX},{sampleY}] = {perlin}\nScale={scale}");
                    noiseVal += perlin * amp;

                    amp *= noise.Persistence;
                    freq *= noise.Lacunarity;
                }

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
