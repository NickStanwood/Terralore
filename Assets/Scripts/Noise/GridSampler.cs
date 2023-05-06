using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GridSampler
{
    public static float SampleSingle(int seed, double x, double y, double z, float radius)
    {
        Vector2 coord = Coordinates.CartesianToCoord((float)x, (float)y, (float)z, radius);
        float lon = coord.x;
        float lat = coord.y;
        float lonGridValue = GridLineValue(lon);
        float latGridValue = GridLineValue(lat);
        return Mathf.Min(lonGridValue, latGridValue);
    }

    private static float GridLineValue(float degree)
    {
        int d = (int)(degree * 180.0f / Mathf.PI);
        if (d == 0)
            return 0.0f;

        if (d % 15 == 0)
            return 0.25f;

        return 1.0f;
    }

    private static bool WithinRange(float value, float target)
    {
        return (value - 0.00001f < target && value + 0.00001f > target);
    }
}
