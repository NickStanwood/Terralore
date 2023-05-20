using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Wind
{
    private const float _WindScale = Mathf.PI / 30;
    public static List<Coord> FindWindOriginBoundary(Coord coord, float windVelocity)
    {
        List<Coord> result = new List<Coord>();
        Coord origin = GetConnectedPoint(coord, windVelocity, false);
        result.Add(new Coord(origin.Lon, origin.Lat + _WindScale));
        result.Add(new Coord(origin.Lon, origin.Lat - _WindScale));

        return result;
    }

    public static Coord FindOrigin(Coord coord, float windVelocity)
    {
        Coord origin = GetConnectedPoint(coord, windVelocity, false);
        return origin;
    }

    public static Coord FindDestination(Coord coord, float windVelocity)
    {
        Coord dest = GetConnectedPoint(coord, windVelocity,true);
        return dest;
    }

    private static Coord GetConnectedPoint(Coord coord, float velocity, bool isDestination)
    {
        int direction = isDestination ? 1 : -1;
        const float outerBand = Mathf.PI / 3;
        const float innerBand = Mathf.PI / 6;
        const float bandSize = Mathf.PI / 6;
        const float bandAngleScale = Mathf.PI / (2 * bandSize);
        float scale = _WindScale * velocity * 10;
        if (coord.Lat >= outerBand)
        {
            //float angle = (lat - outerBand) * bandAngleScale;
            //lon += Mathf.Cos(angle) * scale;
            //lat += Mathf.Sin(angle) * scale;
            coord.Lon -= scale * direction;
        }
        else if (coord.Lat >= innerBand)
        {
            //float angle = (lat - innerBand) * bandAngleScale;
            //lon -= Mathf.Sin(angle) * scale;
            //lat -= Mathf.Cos(angle) * scale;
            coord.Lon += scale * direction;
        }
        else if (coord.Lat >= 0)
        {
            //float angle = (lat) * bandAngleScale;
            //lon += Mathf.Cos(angle) * scale;
            //lat += Mathf.Sin(angle) * scale;
            coord.Lon -= scale * direction;
        }
        else if (coord.Lat >= -innerBand)
        {
            //float angle = (lat + innerBand) * bandAngleScale;
            //lon += Mathf.Sin(angle) * scale;
            //lat -= Mathf.Cos(angle) * scale;
            coord.Lon -= scale * direction;
        }
        else if (coord.Lat >= -outerBand)
        {
            //float angle = (lat + outerBand) * bandAngleScale;
            //lon -= Mathf.Cos(angle) * scale;
            //lat += Mathf.Sin(angle) * scale;
            coord.Lon += scale * direction;
        }
        else
        {
            //float angle = (lat + Mathf.PI / 2) * bandAngleScale;
            //lon += Mathf.Sin(angle) * scale;
            //lat -= Mathf.Cos(angle) * scale;
            coord.Lon += scale * direction;
        }
        return coord;
    }
}
