using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public struct NoiseAttenuationData
{
    public AttenutationType Type;

    [Header("Parabola Attenuation")]
    [Range(0.0f, 1.0f)]
    public float EquatorAttenuation;
    [Range(0.0f, 1.0f)]
    public float PoleAttenuation;

    [Header("Sinusoid and Tangent Attenuation")]
    [Range(0.0f, 1.0f)]
    public float Amplitude;

    [Range(0.0f, 10.0f)]
    public float Frequency;
    [Range(0.0f, 1)]
    public float Offset;

    [Header("Tangent Attenuation")]
    [Range(0.1f, 0.25f)]
    public float AsymptoteCutoff;

    private float EvaluateParabola(float x, float y, float z)
    {
        float r2 = x * x + y * y + z * z;
        float lat2 = y * y;
        float val = (PoleAttenuation - EquatorAttenuation) * lat2 / r2 + EquatorAttenuation;
        if (val < 0.0f) val = 0.0f;
        if (val > 1.0f) val = 1.0f;
        return val;
    }

    private float EvaluateSin(float x, float y, float z)
    {
        float r = Mathf.Sqrt(x * x + y * y + z * z);
        float lat = (y * Mathf.PI) / r;
        return (Mathf.Sin((lat * Frequency) + Offset * Mathf.PI * 2) + 1) * Amplitude / 2;
    }

    private float EvaluateTan(float x, float y, float z)
    {
        float scale = Mathf.Tan(Mathf.PI / 2 + AsymptoteCutoff);
        float r = Mathf.Sqrt(x * x + y * y + z * z);
        float lat = (y * Mathf.PI) / r;

        //check if value is inside the range of the asymptote
        float val = (lat * Frequency) + Offset * Mathf.PI * 2;
        float abs = Mathf.Abs(val);
        if (abs < Mathf.PI / 2 + AsymptoteCutoff && abs > Mathf.PI / 2 - AsymptoteCutoff)
            return 0.5f * Amplitude;

        val = Mathf.Tan(val) * Amplitude / 2;
        val = (val / scale) + 0.5f;

        return val;
    }

    public float Evaluate(float x, float y, float z)
    {
        switch (Type)
        {
            case AttenutationType.Parabola:
                return EvaluateParabola(x, y, z);
            case AttenutationType.Sin:
                return EvaluateSin(x, y, z);
            case AttenutationType.Tan:
                return EvaluateTan(x, y, z);
            default:
                return 1.0f;
        }
    }
}

public enum AttenutationType
{
    None,
    Parabola,
    Sin,
    Tan,
}