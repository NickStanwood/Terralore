using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Noise Attenuation/Sin")]
public class SinAttenuationData : NoiseAttenuationData
{
    [Range(0.0f, 1.0f)]
    public float Amplitude;

    [Range(0.0f, 3.1415f)]
    public float Frequency;

    [Range(0.0f, 3.1415f)]
    public float Offset;

    public override float Evaluate(float x, float y, float z)
    {
        //TODO
    }
}
