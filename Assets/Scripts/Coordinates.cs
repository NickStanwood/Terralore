using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Mercator
{
    public Mercator(float xPercent, float yPercent, int xResolution)
    {
        XSample = (int)(xPercent * xResolution);
        if (XSample >= xResolution)
            XSample -= 1;

        YSample = (int)(yPercent * xResolution/2);
        if(YSample >= xResolution/2)
            YSample -= 1;

        _Resolution = xResolution;
    }

    public Mercator(int xSample, int ySample, int xResolution)
    {
        XSample = xSample;
        YSample = ySample;
        _Resolution = xResolution;
    }

    public int XSample;
    public int YSample;
    public int FlatSample { get { return XSample * _Resolution/2 + YSample; } }

    public float XPercent { get { return (float)XSample / _Resolution; } }
    public float YPercent { get { return (float)YSample / (_Resolution/2); } }

    private int _Resolution;

    public Coord ToCoord(ViewData window)
    {
        Cartesian cart = ToCartesian(window, 1.0f);
        return cart.ToCoord();
    }

    public Cartesian ToCartesian(ViewData window, float radius)
    {
        //get coord of mercator without any window rotations
        float lon = (XPercent - 0.5f) * window.LonAngle;
        float lat = -(YPercent - 0.5f) * window.LatAngle;
        Coord coord = new Coord(lon, lat, radius);

        Cartesian cart = coord.ToCartesian();
        cart.Rotate(window.XRotation, window.YRotation, window.ZRotation);
        return cart;
    }
}

public struct Coord
{
    public float Lon;
    public float Lat;

    private float _Radius;

    public Coord(float lon, float lat, float radius = 1.0f)
    {
        Lon = lon;
        Lat = lat;

        if (Lat > Mathf.PI/2)
        {
            Lat = Mathf.PI/2 - Lat;
            Lon += Mathf.PI;
        }

        if (Lat < -Mathf.PI/2)
        {
            Lat = -Mathf.PI / 2 + Lat;
            Lon += Mathf.PI;
        }

        while (Lon >= Mathf.PI)
            Lon -= Mathf.PI;

        while (Lon < -Mathf.PI)
            Lon += Mathf.PI;

        _Radius = radius;
    }

    public Cartesian ToCartesian()
    {
        float x = _Radius * Mathf.Cos(Lat) * Mathf.Cos(Lon);
        float z = _Radius * Mathf.Cos(Lat) * Mathf.Sin(Lon);
        float y = _Radius * Mathf.Sin(Lat);
        return new Cartesian(x, y, z, _Radius);
    }

    public Mercator ToMercator(ViewData window)
    {
        Cartesian cart = ToCartesian();
        return cart.ToMercator(window);
    }
}

public struct Cartesian
{
    public float X;
    public float Y;
    public float Z;

    private float _Radius;
    public Cartesian(float x, float y, float z, float radius = 1.0f)
    {
        X = x;
        Y = y;
        Z = z;
        _Radius= radius;
    }

    public void Rotate(float xRotation, float yRotation, float zRotation)
    {
        float x, y, z;

        //X Rotation
        y = Y;
        z = Z;
        Y = y * Mathf.Cos(xRotation) + z * Mathf.Sin(xRotation);
        Z = z * Mathf.Cos(xRotation) - y * Mathf.Sin(xRotation);


        //Y Rotation
        x = X;
        z = Z;
        X = x * Mathf.Cos(yRotation) - z * Mathf.Sin(yRotation);
        Z = z * Mathf.Cos(yRotation) + x * Mathf.Sin(yRotation);

        //Z Rotation
        x = X;
        y = Y;
        X = x * Mathf.Cos(zRotation) + y * Mathf.Sin(zRotation);
        Y = y * Mathf.Cos(zRotation) - x * Mathf.Sin(zRotation);
    }

    public Coord ToCoord()
    {
        float lon = Mathf.Atan2(Z, X);
        float lat = Mathf.Asin(Y / _Radius);
        return new Coord(lon, lat, _Radius);
    }

    public Mercator ToMercator(ViewData window)
    {
        //create copy of our cartesian coordinates
        Cartesian copy = new Cartesian(X,Y,Z,_Radius);
        //undo the rotation of the window
        copy.Rotate(-window.XRotation, -window.YRotation, -window.ZRotation);
        Coord coord = copy.ToCoord();
        float xPercent = (coord.Lon / window.LonAngle) + 0.5f;
        float yPercent = (-coord.Lat / window.LatAngle) + 0.5f;
        return new Mercator(xPercent, yPercent, window.Resolution);
    }
}

public static class Coordinates
{
    #region static properties
    public static float MaxLon = Mathf.PI;
    public static float MaxLat = Mathf.PI / 2;


    public static float MinLon = -Mathf.PI;
    public static float MinLat = -Mathf.PI / 2;
    #endregion

    //   screen canvas
    // ==============================
    // | A                          |
    // |                            |
    // |                            |
    // |                            |
    // |                           B|
    // ==============================
    // A:                                       B:
    //     index: 0,0  (x,y)                        index: 128,64  (x,y)
    //     coord: -180, 90 (lon, lat)               coord: 180, -90 (lon, lat)
    //  worldPos: -100.0, 0.0, 50.0 (x,y,z)      worldPos: 100.0, 0.0, -50.0 (x,y,z)

    public static Vector3 CoordToCartesian(float lon, float lat, float radius)
    {
        float x = radius * Mathf.Cos(lat) * Mathf.Cos(lon);
        float z = radius * Mathf.Cos(lat) * Mathf.Sin(lon);
        float y = radius * Mathf.Sin(lat);
        return new Vector3(x, y, z);
    }

    public static Vector2 CartesianToCoord(float x, float y, float z, float radius)
    {
        float lon = Mathf.Atan2(z, x);
        float lat = Mathf.Asin(y / radius);
        return new Vector2(lon, lat);
    }

    public static Vector3 MercatorToCartesian(float xPercent, float yPercent, ViewData window, float radius)
    {
        float lon = (xPercent - 0.5f) * window.LonAngle;
        float lat = -(yPercent - 0.5f) * window.LatAngle;

        Vector3 cart = CoordToCartesian(lon, lat, radius);
        cart = RotateAboutX(cart, window.XRotation);
        cart = RotateAboutY(cart, window.YRotation);
        cart = RotateAboutZ(cart, window.ZRotation);

        return cart;
    }

    public static Vector2 CartesianToMercator(float x, float y, float z, ViewData window, float radius)
    {
        Vector3 cart = new Vector3(x, y, z);
        cart = RotateAboutZ(cart, -window.ZRotation);
        cart = RotateAboutY(cart, -window.YRotation);
        cart = RotateAboutX(cart, -window.XRotation);

        Vector2 coord = CartesianToCoord(cart.x, cart.y, cart.z, radius);
        float lon = coord.x;
        float lat = coord.y;
        float xPercent = (lon / window.LonAngle) + 0.5f;
        float yPercent = (-lat / window.LatAngle) + 0.5f;
        return new Vector2(xPercent, yPercent);
    }

    public static Vector2 MercatorToCoord(float xPercent, float yPercent, ViewData window)
    {
        Vector3 cartesian = MercatorToCartesian(xPercent, yPercent, window, 1.0f);
        return CartesianToCoord(cartesian.x, cartesian.y, cartesian.z, 1.0f);
    }

    public static Vector2 CoordToMercator(float lon, float lat, ViewData window)
    {
        Vector3 cart = CoordToCartesian(lon, lat, 1.0f);
        Vector2 percent = CartesianToMercator(cart.x, cart.y, cart.z, window, 1.0f);
        
        if (percent.y >= 1.0f)
        {
            percent.y = 2.0f - percent.y;
            percent.x += 0.5f;
        }

        if (percent.y < 0.0f)
        {
            percent.y *= -1.0f;
            percent.x += 0.5f;
        }

        while (percent.x >= 1.0f)
            percent.x -= 1.0f;

        while (percent.x < 0.0f)
            percent.x += 1.0f;

        return percent;
    }

    public static Vector3 RotateAboutX(Vector3 cartesian, float rotation)
    {
        float y = cartesian.y;
        float z = cartesian.z;

        cartesian.y = y * Mathf.Cos(rotation) + z * Mathf.Sin(rotation);
        cartesian.z = z * Mathf.Cos(rotation) - y * Mathf.Sin(rotation);
        
        return cartesian;
    }

    public static Vector3 RotateAboutY(Vector3 cartesian, float rotation)
    {
        float x = cartesian.x;
        float z = cartesian.z;

        cartesian.x = x * Mathf.Cos(rotation) - z * Mathf.Sin(rotation);
        cartesian.z = z * Mathf.Cos(rotation) + x * Mathf.Sin(rotation);

        return cartesian;
    }

    public static Vector3 RotateAboutZ(Vector3 cartesian, float rotation)
    {
        float x = cartesian.x;
        float y = cartesian.y;

        cartesian.x = x * Mathf.Cos(rotation) + y * Mathf.Sin(rotation);
        cartesian.y = y * Mathf.Cos(rotation) - x * Mathf.Sin(rotation);

        return cartesian;
    }
}
