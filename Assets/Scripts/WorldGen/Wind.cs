using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Wind
{
    public static Vector2 FindOrigin(Vector2 coord)
    {
        float lon = coord.x;
        float lat = coord.y;
        float scale = Mathf.PI / 60;
        const float outerBand = Mathf.PI / 3;
        const float innerBand = Mathf.PI / 6;
        const float bandSize = Mathf.PI / 6;
        const float bandAngleScale = Mathf.PI / (2 * bandSize);
        if (lat > outerBand)
        {
            float angle = (lat - outerBand) * bandAngleScale;
            lon += Mathf.Cos(angle) * scale;
            lat += Mathf.Sin(angle) * scale;
        }
        else if (lat > innerBand)
        {
            float angle = (lat - innerBand) * bandAngleScale;
            lon -= Mathf.Sin(angle) * scale;
            lat -= Mathf.Cos(angle) * scale;
        }
        else if (lat >= 0)
        {
            float angle = (lat) * bandAngleScale;
            lon += Mathf.Cos(angle) * scale;
            lat += Mathf.Sin(angle) * scale;
        }
        else if (lat > -innerBand)
        {
            float angle = (lat + innerBand) * bandAngleScale;
            lon += Mathf.Sin(angle) * scale;
            lat -= Mathf.Cos(angle) * scale;
        }
        else if (lat > -outerBand)
        {
            float angle = (lat + outerBand) * bandAngleScale;
            lon -= Mathf.Cos(angle) * scale;
            lat += Mathf.Sin(angle) * scale;
        }
        else
        {
            float angle = (lat + Mathf.PI / 2) * bandAngleScale;
            lon += Mathf.Sin(angle) * scale;
            lat -= Mathf.Cos(angle) * scale;
        }
        return new Vector2(lon, lat);
    }

    public static float Velocity(Vector2 coord)
    {
        //TODO:
        return 1.0f;
    }
}
