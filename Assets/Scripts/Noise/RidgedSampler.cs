using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RidgedSampler : PerlinSampler
{
    public RidgedSampler(NoiseData data) : base(data) { }

    protected override float SampleSingle(int seed, double x, double y)
    {
        float val = base.SampleSingle(seed, x, y);
        return 1.0f - Mathf.Abs((val - 0.5f) * 2.0f);
    }

    protected override float SampleSingle(int seed, double x, double y, double z)
    {
        float val = base.SampleSingle(seed, x, y, z);
        return 1.0f - Mathf.Abs((val - 0.5f) * 2.0f);
    }
}
