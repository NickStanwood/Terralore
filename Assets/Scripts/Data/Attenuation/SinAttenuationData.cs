using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Noise Attenuation/Sin")]
public class SinAttenuationData : NoiseAttenuationData
{
    [Range(0.0f, 1.0f)]
    public float Amplitude;

    [Range(0.0f, 10.0f)]
    public float Frequency;

    [Range(0.0f, 1)]
    public float Offset;

    public override float Evaluate(float x, float y, float z)
    {
        float r = Mathf.Sqrt(x * x + y * y + z * z);
        float lat = (y*Mathf.PI) / r;
        return (Mathf.Sin((lat * Frequency) + Offset*Mathf.PI*2) + 1) * Amplitude/2;
    }
}
