using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NoiseParams
{
    [Range(0.0001f, 1.0f)]
    public float Frequency;     //the larger the value the faster the noise changes

    [Range(1, 30)]
    public int Octaves;         //how many iterations of noise

    [Range(0.000f, 1.0f)]
    public float Persistence;   //how fast the amplitude decreases with each octave

    [Range(1.0f, 10.0f)]
    public float Lacunarity;    //how fast the scale of each octave decreases
}
