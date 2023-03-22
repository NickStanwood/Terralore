using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Coordinates
{
    #region static properties
    public static float MaxLon = Mathf.PI;
    public static float MaxLat = Mathf.PI / 2;


    public static float MinLon = -Mathf.PI;
    public static float MinLat = -Mathf.PI / 2;
    #endregion


    public static Vector3 CoordToCartesian(float lon, float lat, float radius)
    {
        float x = radius * Mathf.Cos(lat) * Mathf.Cos(lon);
        float z = radius * Mathf.Cos(lat) * Mathf.Sin(lon);
        float y = -radius * Mathf.Sin(lat);
        return new Vector3(x, y, z);
    }

    public static Vector2 CartesianToCoord(float x, float y, float z, float radius)
    {
        float lon = Mathf.Atan2(y, x);
        float lat = Mathf.Asin(z / radius);
        return new Vector3(lon, lat);
    }

    public static Vector3 MercatorToCartesian(float xPercent, float yPercent, ViewData window, float radius)
    {
        float lon = (xPercent - 0.5f) * window.LonAngle;
        float lat = (yPercent - 0.5f) * window.LatAngle;

        Vector3 cart = CoordToCartesian(lon, lat, radius);
        cart = RotateAboutX(cart, window.XRotation);
        cart = RotateAboutY(cart, window.YRotation);
        cart = RotateAboutZ(cart, window.ZRotation);

        return cart;
        //return new Vector3(cart.x, cart.y, cart.z);
    }

    public static Vector2 MercatorToCoord(int xIndex, int yIndex, ViewData window)
    {
        Vector3 cartesian = MercatorToCartesian(xIndex, yIndex, window, 1.0f);
        return CartesianToCoord(cartesian.x, cartesian.y, cartesian.z, 1.0f);
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
