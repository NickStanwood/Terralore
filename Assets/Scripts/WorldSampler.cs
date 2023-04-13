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
    public float[,] WindMap { get; set; }

    private float _WorldHeightMax;
    private float _WorldHeightMin;
    private float _LocalHeightMax;
    private float _LocalHeightMin;

    public WorldSample SampleFromCoord(float lon, float lat)
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
            WorldPos = ConvertMapIndexToWorldPos(x, y),
            Height = HeightMap[x,y],
            Heat = HeatMap[x,y],
            //Wind = WindMap[x,y]
        };
    }

    public WorldSample SampleFromIndex(int xIndex, int yIndex)
    {
        //get Lon & Lat
        float xPercent = (float)xIndex / (float)HeightMap.GetLength(0);
        float yPercent = (float)yIndex / (float)HeightMap.GetLength(1);
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
            //Wind = WindMap[xIndex, yIndex]
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

    private Vector3 ConvertMapIndexToWorldPos(int xIndex, int yIndex)
    {
        //get value between 0 - 1. 0 being world min height. 1 being worldmax height
        float noiseHeight = (HeightMap[xIndex, yIndex] - _WorldHeightMin) / (_WorldHeightMax - _WorldHeightMin);
        noiseHeight = Mathf.Max(noiseHeight, OceanLevel);

        float minNoiseHeight = (_LocalHeightMin - _WorldHeightMin) / (_WorldHeightMax - _WorldHeightMin);
        minNoiseHeight = Mathf.Max(minNoiseHeight, OceanLevel);

        float meshY = (noiseHeight - minNoiseHeight) * HeightScale;
        float meshX = xIndex * MapScale;
        float meshZ = yIndex * MapScale;
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
    public float Wind;
}
