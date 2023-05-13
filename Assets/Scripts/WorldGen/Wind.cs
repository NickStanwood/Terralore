using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Wind
{
    private const float _WindScale = Mathf.PI / 30;
    public static List<Coord> FindWindOriginBoundary(Coord coord)
    {
        List<Coord> result = new List<Coord>();
        Coord origin = GetConnectedPoint(coord, false);
        result.Add(new Coord(origin.Lon, origin.Lat + _WindScale));
        result.Add(new Coord(origin.Lon, origin.Lat - _WindScale));

        //TODO find a way to scale these origins points based on the velocity 

        return result;
    }

    public static Coord FindOrigin(Coord coord)
    {
        Coord origin = GetConnectedPoint(coord, false);
        return origin;
    }

    public static Coord FindDestination(Coord coord)
    {
        Coord dest = GetConnectedPoint(coord, true);
        return dest;
    }

    public static float Velocity(Coord coord)
    {
        //TODO:
        return 1.0f;
    }
    private static Coord GetConnectedPoint(Coord coord, bool isDestination)
    {
        int direction = isDestination ? 1 : -1;
        const float outerBand = Mathf.PI / 3;
        const float innerBand = Mathf.PI / 6;
        const float bandSize = Mathf.PI / 6;
        const float bandAngleScale = Mathf.PI / (2 * bandSize);
        if (coord.Lat >= outerBand)
        {
            //float angle = (lat - outerBand) * bandAngleScale;
            //lon += Mathf.Cos(angle) * scale;
            //lat += Mathf.Sin(angle) * scale;
            coord.Lon -= _WindScale * direction;
        }
        else if (coord.Lat >= innerBand)
        {
            //float angle = (lat - innerBand) * bandAngleScale;
            //lon -= Mathf.Sin(angle) * scale;
            //lat -= Mathf.Cos(angle) * scale;
            coord.Lon += _WindScale * direction;
        }
        else if (coord.Lat >= 0)
        {
            //float angle = (lat) * bandAngleScale;
            //lon += Mathf.Cos(angle) * scale;
            //lat += Mathf.Sin(angle) * scale;
            coord.Lon -= _WindScale * direction;
        }
        else if (coord.Lat >= -innerBand)
        {
            //float angle = (lat + innerBand) * bandAngleScale;
            //lon += Mathf.Sin(angle) * scale;
            //lat -= Mathf.Cos(angle) * scale;
            coord.Lon -= _WindScale * direction;
        }
        else if (coord.Lat >= -outerBand)
        {
            //float angle = (lat + outerBand) * bandAngleScale;
            //lon -= Mathf.Cos(angle) * scale;
            //lat += Mathf.Sin(angle) * scale;
            coord.Lon += _WindScale * direction;
        }
        else
        {
            //float angle = (lat + Mathf.PI / 2) * bandAngleScale;
            //lon += Mathf.Sin(angle) * scale;
            //lat -= Mathf.Cos(angle) * scale;
            coord.Lon += _WindScale * direction;
        }
        return coord;
    }
}
