using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class WorldSampler : UpdatableData
{
    [Header("Display Data")]
    public ViewData window;
    [Range(0.001f, 10.0f)]
    public float HeightScale;
    [Range(0.001f, 10.0f)]
    public float MapScale;

    [Header("World Data")]
    [Range(0.0f, 1.0f)]
    public float OceanLevel;

    [HideInInspector]
    public float[,] HeightMap { get; set; }
    [HideInInspector]
    public float[,] HeatMap { get; set; }
    [HideInInspector]
    public float[,] WindVelocityMap { get; set; }
    [HideInInspector]
    public float[,] WindRotationMap { get; set; }

    private float _WorldHeightMax;
    private float _WorldHeightMin;
    private float _LocalHeightMax;
    private float _LocalHeightMin;

    public WorldSample SampleFromCoord(float lon, float lat)
    {
        Vector2 percent = Coordinates.CoordToMercator(lon, lat, window);
        while(percent.x >= 1.0f)
            percent.x -= 1.0f;

        while (percent.x < 0.0f)
            percent.x += 1.0f;

        while (percent.y >= 1.0f)
            percent.y -= 1.0f;

        while (percent.y < 0.0f)
            percent.y += 1.0f;

        int x = (int)(percent.x * MapIndexWidth());
        int y = (int)(percent.y * MapIndexHeight());
        
        return new WorldSample
        {
            xIndex = x,
            yIndex = y,
            Longitude = lon,
            Latitude = lat,
            WorldPos = ConvertMapIndexToWorldPos(x, y),
            Height = HeightMap[x,y],
            Heat = HeatMap[x,y],
            WindRotation = WindRotationMap[x, y] * Mathf.PI * 2,
            WindVelocity = WindVelocityMap[x, y]
        };
    }

    public WorldSample SampleFromIndex(int xIndex, int yIndex)
    {
        //get Lon & Lat
        float xPercent = (float)xIndex / (float)MapIndexWidth();
        float yPercent = (float)yIndex / (float)MapIndexHeight();
        Vector2 coords = Coordinates.MercatorToCoord(xPercent, yPercent, window);

        return new WorldSample
        {
            xIndex = xIndex,
            yIndex = yIndex,
            Longitude = coords.x,
            Latitude = coords.y,
            WorldPos = ConvertMapIndexToWorldPos(xIndex, yIndex),
            Height = HeightMap[xIndex, yIndex],
            Heat = HeatMap[xIndex, yIndex],
            WindRotation = WindRotationMap[xIndex, yIndex]*Mathf.PI*2,
            WindVelocity = WindVelocityMap[xIndex, yIndex]
        };
    }

    public bool WorldHeightsSet()
    {
        return _WorldHeightMax != 0.0f || _WorldHeightMin != 0.0f;
    }

    public void SetWorldHeights(float worldMinHeight, float worldMaxHeight)
    {
        _WorldHeightMin = worldMinHeight;
        _WorldHeightMax = worldMaxHeight;
    }

    public void SetLocalHeights(float localMinHeight, float localMaxHeight)
    {
        _LocalHeightMin = localMinHeight;
        _LocalHeightMax = localMaxHeight;
    }

    public float MinMeshHeight()
    {
        return NoiseValueToWorldHeight(_WorldHeightMin);
    }

    public float MaxMeshHeight()
    {
        return NoiseValueToWorldHeight(_WorldHeightMax);
    }

    public int MapIndexWidth()
    {
        return HeightMap.GetLength(0);
    }

    public int MapIndexHeight()
    {
        return HeightMap.GetLength(1);
    }

    private Vector3 ConvertMapIndexToWorldPos(int xIndex, int yIndex)
    {
        //get value between 0 - 1. 0 being world min height. 1 being worldmax height
        float noiseHeight = (HeightMap[xIndex, yIndex] - _WorldHeightMin) / (_WorldHeightMax - _WorldHeightMin);
        noiseHeight = Mathf.Max(noiseHeight, OceanLevel);

        float minNoiseHeight = (_LocalHeightMin - _WorldHeightMin) / (_WorldHeightMax - _WorldHeightMin);
        minNoiseHeight = Mathf.Max(minNoiseHeight, OceanLevel);

        //calculate mesh height so the lowest point in the local window is at 0
        float meshY = (noiseHeight - minNoiseHeight) * HeightScale;

        //calculate x mesh location so the center of the map is at 0
        float topLeftX = (MapIndexWidth() - 1) / -2f;
        float meshX = (xIndex + topLeftX) * MapScale;

        //calculate z mesh location so the center of the map is at 0
        float topLeftZ = (MapIndexHeight() - 1) / 2f;
        float meshZ = (topLeftZ - yIndex) * MapScale;

        return new Vector3(meshX, meshY, meshZ);
    }

    private float NoiseValueToWorldHeight(float value)
    {
        //get value between 0 - 1. 0 being world min height. 1 being worldmax height
        float noiseHeight = (value - _WorldHeightMin) / (_WorldHeightMax - _WorldHeightMin);
        noiseHeight = Mathf.Max(noiseHeight, OceanLevel);

        float minNoiseHeight = (_LocalHeightMin - _WorldHeightMin) / (_WorldHeightMax - _WorldHeightMin);
        minNoiseHeight = Mathf.Max(minNoiseHeight, OceanLevel);

        float meshHeight = (noiseHeight - minNoiseHeight) * HeightScale;

        return meshHeight;
    }
}

public struct WorldSample
{
    public int xIndex;
    public int yIndex;

    public float Longitude;
    public float Latitude;

    public Vector3 WorldPos;

    public float Height;
    public float Heat;
    public float WindRotation;
    public float WindVelocity;
}
