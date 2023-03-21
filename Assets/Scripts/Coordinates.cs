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

    public static Vector3 MercatorToCartesian(int xIndex, int yIndex, ViewData window, float radius)
    {
        float xPercent = (float)xIndex / window.LonResolution;
        float yPercent = (float)yIndex / window.LatResolution;

        xPercent = (xPercent - 0.5f);
        yPercent = (yPercent - 0.5f);

        float lon = xPercent * window.LonAngle;
        float lat = yPercent * window.LatAngle;

        Vector3 cart = CoordToCartesian(lon, lat, radius);

        //adjust for latitude rotation
        float xSphere = cart.x;
        float ySphere = cart.y * Mathf.Cos(window.LatOffset) + cart.z * Mathf.Sin(window.LatOffset);
        float zSphere = cart.z * Mathf.Cos(window.LatOffset) - cart.y * Mathf.Sin(window.LatOffset);


        cart = new Vector3(xSphere, ySphere, zSphere);

        //adjust for longitutde rotation
        xSphere = cart.x * Mathf.Cos(window.LonOffset) - cart.z * Mathf.Sin(window.LonOffset);
        ySphere = cart.y;
        zSphere = cart.z * Mathf.Cos(window.LonOffset) + cart.x * Mathf.Sin(window.LonOffset);

        return new Vector3(xSphere, ySphere, zSphere);
        //return new Vector3(cart.x, cart.y, cart.z);
    }

    public static Vector2 MercatorToCoord(int xIndex, int yIndex, ViewData window)
    {
        Vector3 cartesian = MercatorToCartesian(xIndex, yIndex, window, 1.0f);
        return CartesianToCoord(cartesian.x, cartesian.y, cartesian.z, 1.0f);
    }
}
