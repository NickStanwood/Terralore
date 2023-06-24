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
                Mercator merc = new Mercator(x, y, window.LonResolution);
                Cartesian cart = merc.ToCartesian(window, worldRadius);

                float noiseVal = noise.Sample(cart.X, cart.Y, cart.Z);

                if (noiseVal < localMinNoise)
                    localMinNoise = noiseVal;

                if (noiseVal > localMaxNoise)
                    localMaxNoise = noiseVal;

                noiseMap[x * window.LatResolution + y] = noiseVal;
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
        foreach (var job in noiseJobs.Values)
        {
            job.noiseMap.Dispose();
        }
    }
}