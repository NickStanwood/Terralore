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

        float lon = xPercent * (MaxLon - MinLon) - MinLon;
        float lat = yPercent * (MaxLat - MinLat) - MinLat;

        Vector3 cart = CoordToCartesian(lon, lat, radius);

        //adjust for latitude rotation
        float xSphere = cart.x;
        float ySphere = cart.y * Mathf.Cos(window.LatTop) + cart.z * Mathf.Sin(window.LatTop);
        float zSphere = cart.z * Mathf.Cos(window.LatTop) - cart.y * Mathf.Sin(window.LatTop);


        cart = new Vector3(xSphere, ySphere, zSphere);

        //adjust for longitutde rotation
        xSphere = cart.x * Mathf.Cos(window.LonLeft) - cart.z * Mathf.Sin(window.LonLeft);
        ySphere = cart.y;
        zSphere = cart.z * Mathf.Cos(window.LonLeft) + cart.x * Mathf.Sin(window.LonLeft);

        return new Vector3(xSphere, ySphere, zSphere);
    }
}
