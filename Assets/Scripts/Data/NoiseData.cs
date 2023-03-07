using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class NoiseData : UpdatableData
{
    public int Seed;

    NoiseSamplingType Type;

    [Range(0.0001f, 0.01f)]
    public float Frequency;     //the larger the value the faster the noise changes

    [Range(1, 30)]
    public int Octaves;         //how many iterations of noise

    [Range(0.000f, 1.0f)]
    public float Persistence;   //how fast the amplitude decreases with each octave

    [Range(1.0f, 10.0f)]
    public float Lacunarity;    //how fast the scale of each octave decreases
}

public enum NoiseSamplingType
{
    Perlin,
    Cellular,
}