using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSampler : NoiseSampler
{
    private float Radius;
    public GridSampler(NoiseData noise, float worldRadius) : base(noise) 
    {
        Radius = worldRadius;
    }

    protected override float SampleSingle(int seed, double x, double y)
    {
        return 0.0f;
    }

    protected override float SampleSingle(int seed, double x, double y, double z)
    {
        Vector2 coord = Coordinates.CartesianToCoord((float)x, (float)y, (float)z, Radius);
        float lon = coord.x;
        float lat = coord.y;
        float lonGridValue = GridLineValue(lon);
        //string debugMsg = "== sample == \n";
        //debugMsg += $"xyz: ({x.ToString("N3")},{y},{z})\n";
        //debugMsg += $"lon: {(int)(lon * 180.0f / Mathf.PI)} -> {lonGridValue}\n";

        float latGridValue = GridLineValue(lat);
        //debugMsg += $"lat: {(int)(lat * 180.0f / Mathf.PI)} -> {latGridValue}\n";
        //Debug.Log(debugMsg);
        return Mathf.Min(lonGridValue, latGridValue);
    }

    private float GridLineValue(float degree)
    {
        int d = (int)(degree * 180.0f / Mathf.PI);
        if (d == 0)
            return 0.0f;

        if (d % 15 == 0)
            return 0.25f;

        return 1.0f;
    }

    private bool WithinRange(float value, float target)
    {
        return (value - 0.00001f < target && value + 0.00001f > target);
    }
}
