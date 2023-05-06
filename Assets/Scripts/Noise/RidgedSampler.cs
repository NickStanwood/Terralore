using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RidgedSampler
{
    public static float SampleSingle(int seed, double x, double y, float sigmoidGain)
    {
        float val = PerlinSampler.SampleSingle(seed, x, y);
        float sigmoid = 1.0f / (1.0f + Mathf.Exp(-sigmoidGain * (val - 0.5f)));
        return 1.0f - Mathf.Abs((sigmoid - 0.5f) * 2.0f);
    }

    public static float SampleSingle(int seed, double x, double y, double z, float sigmoidGain)
    {
        float val = PerlinSampler.SampleSingle(seed, x, y, z);
        float sigmoid = 1.0f / (1.0f + Mathf.Exp(-sigmoidGain * (val - 0.5f)));
        return 1.0f - Mathf.Abs((sigmoid - 0.5f) * 2.0f);
    }
}
