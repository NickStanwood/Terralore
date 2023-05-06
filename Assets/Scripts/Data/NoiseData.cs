using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public struct NoiseData
{
    public NoiseType Type;
    public int Seed;

    [Range(0.0f, 1.0f)]
    public float Amplitude;     //sampled noise values garaunteed to be between 0 and amplitude

    [Range(0.0001f, 0.01f)]
    public float Frequency;     //the larger the value the faster the noise changes

    [Range(1, 30)]
    public int Octaves;         //how many iterations of noise

    [Range(0.000f, 1.0f)]
    public float Persistence;   //how fast the amplitude decreases with each octave

    [Range(1.0f, 10.0f)]
    public float Lacunarity;    //how fast the scale of each octave decreases

    public NoiseAttenuationData AttenuationCurve;

    [Range(1.0f, 100.0f)]
    public float RidgeSteepness;    //used for ridged noise only

    public float Sample(double x, double y)
    {
        int seed = Seed;
        float sum = 0f;
        float max = 0f;
        float amp = Amplitude;
        double freq = Frequency;

        for (int i = 0; i < Octaves; i++)
        {
            float noise = 0.0f;
            if (Type == NoiseType.Ridged)
                noise = RidgedSampler.SampleSingle(seed++, x * freq, y * freq, RidgeSteepness) * amp;
            else
                noise = PerlinSampler.SampleSingle(seed++, x * freq, y * freq) * amp;

            sum += noise;
            max += amp;

            amp *= Persistence;
            freq *= Lacunarity;
        }

        float noiseVal = sum / max;

        return noiseVal;
    }

    public float Sample(double x, double y, double z)
    {
        int seed = Seed;
        float sum = 0f;
        float max = 0f;
        float amp = 1.0f;
        double freq = Frequency;

        for (int i = 0; i < Octaves; i++)
        {
            float noise = 0.0f;
            if (Type == NoiseType.Ridged)
                noise = RidgedSampler.SampleSingle(seed++, x * freq, y * freq, z * freq, RidgeSteepness) * amp;
            else
                noise = PerlinSampler.SampleSingle(seed++, x * freq, y * freq, z * freq) * amp;
            sum += noise;
            max += amp;

            amp *= Persistence;
            freq *= Lacunarity;
        }

        float noiseVal = sum * Amplitude / max;

        noiseVal *= AttenuationCurve.Evaluate((float)x, (float)y, (float)z);

        return noiseVal;
    }
}

public enum NoiseType { Perlin, Ridged, Grid}