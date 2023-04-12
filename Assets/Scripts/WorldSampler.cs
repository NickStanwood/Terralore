using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class WorldSampler : UpdatableData
{
    public ViewData window;

    public float[,] HeightMap { get; set; }
    public float[,] HeatMap { get; set; }
    public float[,] WindMap { get; set; }

    public WorldSample Sample(float lon, float lat)
    {
        Vector2 percent = Coordinates.CoordToMercator(lon, lat, window);
        int x = (int)(percent.x * HeightMap.GetLength(0));
        int y = (int)(percent.y * HeightMap.GetLength(1));
        
        return new WorldSample
        {
            xIndex = x,
            yIndex = y,
            Longitude = lon,
            Latitude = lat,
            Height = HeightMap[x,y],
            Heat = HeatMap[x,y],
            Wind = WindMap[x,y]
        };
    }
}

public struct WorldSample
{
    public int xIndex;
    public int yIndex;

    public float Longitude;
    public float Latitude;

    public float Height;
    public float Heat;
    public float Wind;
}
