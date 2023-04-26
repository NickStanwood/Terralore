using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;

public struct NoiseGenJob : IJob
{
    //INPUT
    public NoiseData noise;
    public ViewData window;
    public float worldRadius;

    //OUTPUT
    public float[,] map;
    public float localMax;
    public float localMin;

    public void Execute()
    {
        map = Noise.GenerateNoiseMap(noise, window, worldRadius, out localMin, out localMax);
    }
}


public static class Noise
{
    public static JobHandle GenerateNoiseMapJob(NoiseData noise, ViewData window, float worldRadius)
    {
        NoiseGenJob job = new NoiseGenJob
        {
            noise = noise,
            window = window,
            worldRadius = worldRadius
        }

        return job.Schedule();
    }

    public static float[,] GenerateNoiseMap(NoiseData noise, ViewData window, float worldRadius)
    {
        float max, min;
        return GenerateNoiseMap(noise, window, worldRadius, out min, out max);
    }

    public static float[,] GenerateNoiseMap(NoiseData noise, ViewData window, float worldRadius, out float localMinNoise, out float localMaxNoise)
    {
        float[,] noiseMap = new float[window.LonResolution, window.LatResolution];

        double lonSampleFreq = window.LonAngle / window.LonResolution;
        double latSampleFreq = window.LatAngle / window.LatResolution;

        float absoluteMaxNoise = GetMaxNoise(noise);

        localMinNoise = float.MaxValue;
        localMaxNoise = float.MinValue;

        NoiseSampler sampler = null;
        if(noise.Type == NoiseType.Ridged)
            sampler = new RidgedSampler(noise);
        else if(noise.Type == NoiseType.Grid)
            sampler = new GridSampler(noise, worldRadius);
        else
            sampler = new PerlinSampler(noise);

        for (int y = 0; y < window.LatResolution; y++)
        {
            for (int x = 0; x < window.LonResolution; x++)
            {
                float xPercent = (float)x / window.LonResolution;
                float yPercent = (float)y / window.LatResolution;
                Vector3 c = Coordinates.MercatorToCartesian(xPercent, yPercent, window, worldRadius);

                float noiseVal = sampler.Sample(c.x, c.y, c.z);

                if (noiseVal < localMinNoise)
                    localMinNoise = noiseVal;

                if (noiseVal > localMaxNoise)
                    localMaxNoise = noiseVal;

                noiseMap[x, y] = noiseVal;
            }
        }

        //normalize max and min vlaues as well as noisemap
        localMaxNoise = localMaxNoise / absoluteMaxNoise;
        localMinNoise = localMinNoise / absoluteMaxNoise;
        return Normalize(noiseMap, noise.Amplitude, absoluteMaxNoise, 0.0f);
    }


    public static float[,] GenerateNoiseMap(List<NoiseData> noiseLayers, ViewData window, float worldRadius, out float localMinNoise, out float localMaxNoise)
    {
        List<float[,]> noiseMaps = new List<float[,]>();
        float absoluteMaxNoise = 0.0f;
        foreach(NoiseData noise in noiseLayers)
        {
            noiseMaps.Add(GenerateNoiseMap(noise, window, worldRadius));
            absoluteMaxNoise += noise.Amplitude;
        }

        return Combine(noiseMaps, out localMinNoise, out localMaxNoise);
    }

    public static float[,] Combine(List<float[,]> noiseLayers, out float localMinNoise, out float localMaxNoise)
    {
        int width = noiseLayers[0].GetLength(0);
        int height = noiseLayers[0].GetLength(1);

        localMinNoise = float.MaxValue;
        localMaxNoise = float.MinValue;

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

                if (noiseVal < localMinNoise)
                    localMinNoise = noiseVal;

                if (noiseVal > localMaxNoise)
                    localMaxNoise = noiseVal;

                noiseMap[x, y] = noiseVal;
            }
        }

        return noiseMap;
    }

    public static float[,] Normalize(float[,] noiseMap, float amp, float max, float min)
    {
        if (max == min)
            return noiseMap;

        for (int x = 0; x < noiseMap.GetLength(0); x++)
        {
            for (int y = 0; y < noiseMap.GetLength(1); y++)
            {
                noiseMap[x, y] = ((noiseMap[x, y] - min) / (max - min))*amp;
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
