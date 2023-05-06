using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;

[BurstCompile]
public struct NoiseGenJob : IJob
{
    //INPUT
    [ReadOnly]
    public NoiseData noise;
    [ReadOnly]
    public ViewData window;
    [ReadOnly]
    public float worldRadius;

    //OUTPUT
    public NativeArray<float> noiseMap;
    public float localMaxNoise;
    public float localMinNoise;

    public void Execute()
    {
        double lonSampleFreq = window.LonAngle / window.LonResolution;
        double latSampleFreq = window.LatAngle / window.LatResolution;
                
        localMinNoise = float.MaxValue;
        localMaxNoise = float.MinValue;

        for (int y = 0; y < window.LatResolution; y++)
        {
            for (int x = 0; x < window.LonResolution; x++)
            {
                float xPercent = (float)x / window.LonResolution;
                float yPercent = (float)y / window.LatResolution;
                Vector3 c = Coordinates.MercatorToCartesian(xPercent, yPercent, window, worldRadius);

                float noiseVal = noise.Sample(c.x, c.y, c.z);

                if (noiseVal < localMinNoise)
                    localMinNoise = noiseVal;

                if (noiseVal > localMaxNoise)
                    localMaxNoise = noiseVal;

                noiseMap[x*window.LatResolution + y] = noiseVal;
            }
        }
    }
}


public class NoiseJobArray
{
    Dictionary<string, NoiseGenJob> noiseJobs = new Dictionary<string, NoiseGenJob>();
    public void Add(string name, NoiseData noise, ViewData window, float worldRadius)
    {
        NoiseGenJob job = new NoiseGenJob
        {
            noise = noise,
            window = window,
            worldRadius = worldRadius,
            noiseMap = new NativeArray<float>(window.LonResolution * window.LatResolution, Allocator.Persistent)
        };

        noiseJobs.Add(name, job);
    }

    public void RunAll()
    {
        NativeList<JobHandle> handles = new NativeList<JobHandle>(Allocator.Temp);
        foreach (NoiseGenJob job in noiseJobs.Values)
        {
            handles.Add(job.Schedule());
        }
        JobHandle.CompleteAll(handles);
        handles.Dispose();
    }

    public void RunOne(string name)
    {
        JobHandle handle = noiseJobs[name].Schedule();
        handle.Complete();

    }

    public float LocalMin(string name)
    {
        return noiseJobs[name].localMinNoise;
    }

    public float LocalMax(string name)
    {
        return noiseJobs[name].localMaxNoise;
    }

    public float[] CopyNoise(string name)
    {
        int size = noiseJobs[name].window.LonResolution * noiseJobs[name].window.LatResolution;
        float[] noise = new float[size];

        NativeArray<float>.Copy(noiseJobs[name].noiseMap, noise);
        return noise;
    }

    public void Dispose()
    {
        foreach(var job in noiseJobs.Values)
        {
            job.noiseMap.Dispose();
        }
    }
}

public static class Noise
{    
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

        localMinNoise = float.MaxValue;
        localMaxNoise = float.MinValue;

        for (int y = 0; y < window.LatResolution; y++)
        {
            for (int x = 0; x < window.LonResolution; x++)
            {
                float xPercent = (float)x / window.LonResolution;
                float yPercent = (float)y / window.LatResolution;
                Vector3 c = Coordinates.MercatorToCartesian(xPercent, yPercent, window, worldRadius);

                float noiseVal = noise.Sample(c.x, c.y, c.z);

                if (noiseVal < localMinNoise)
                    localMinNoise = noiseVal;

                if (noiseVal > localMaxNoise)
                    localMaxNoise = noiseVal;

                noiseMap[x, y] = noiseVal;
            }
        }

        return noiseMap;
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
}
