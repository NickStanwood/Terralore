using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu()]
public class NoiseData : UpdatableData
{
    public NoiseType Type = NoiseType.Perlin;
    public int Seed;

    [Range(0.0f, 1.0f)]
    public float Amplitude= 1.0f;     //sampled noise values garaunteed to be between 0 and amplitude

    [Range(0.0001f, 0.01f)]
    public float Frequency = 0.0001f;     //the larger the value the faster the noise changes

    [Range(1, 30)]
    public int Octaves = 1;         //how many iterations of noise

    [Range(0.000f, 1.0f)]
    public float Persistence = 0.5f;   //how fast the amplitude decreases with each octave

    [Range(1.0f, 10.0f)]
    public float Lacunarity = 2.0f;    //how fast the scale of each octave decreases

    public NoiseAttenuationData AttenuationCurve;

    [Range(1.0f, 100.0f)]
    public float RidgeSteepness;    //used for ridged noise only

    protected override void OnValidate()
    {
        if (AttenuationCurve != null)
        {
            AttenuationCurve.OnValuesUpdated.RemoveListener(NotifyOfUpdatedValues);
            AttenuationCurve.OnValuesUpdated.AddListener(NotifyOfUpdatedValues);
        }
        base.OnValidate();
    }
}

public enum NoiseType { Perlin, Ridged, Grid}