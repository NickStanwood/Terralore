using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RidgedSampler : PerlinSampler
{
    private float sigmoidGain;

    public RidgedSampler(NoiseData data) : base(data) 
    {
        sigmoidGain = data.RidgeSteepness;
    }

    protected override float SampleSingle(int seed, double x, double y)
    {
        float val = base.SampleSingle(seed, x, y);
        float sigmoid = 1.0f / (1.0f + Mathf.Exp(-sigmoidGain * (val - 0.5f)));
        return 1.0f - Mathf.Abs((sigmoid - 0.5f) * 2.0f);
    }

    protected override float SampleSingle(int seed, double x, double y, double z)
    {
        float val = base.SampleSingle(seed, x, y, z);
        float sigmoid = 1.0f / (1.0f + Mathf.Exp(-sigmoidGain * (val - 0.5f)));
        return 1.0f - Mathf.Abs((sigmoid - 0.5f) * 2.0f);
    }
}
