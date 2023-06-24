using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

[CreateAssetMenu()]
public class WorldSampler : UpdatableData
{
    [Header("Display Data")]
    [Range(0.001f, 40.0f)]
    public float HeightScale;
    [Range(0.001f, 10.0f)]
    public float MapScale;
    [SerializeField] private ViewData Window;

    [Header("World Data")]
    public WorldData WorldData;

    public WorldSample SampleFromCoord(float lon, float lat)
    {
        Coord coord = new Coord(lon, lat);
        Mercator merc = coord.ToMercator(Window);

        return new WorldSample
        {
            Index = merc,
            Coord = coord,
            WorldPos = ConvertMapIndexToWorldPos(merc),
            Height = _HeightMap[merc.FlatSample] + _MountainMap[merc.FlatSample],
            Heat = _HeatMap[merc.FlatSample],
            Moisture = _MoistureMap[merc.FlatSample]
        };
    }

    public WorldSample SampleFromIndex(int xIndex, int yIndex)
    {
        Mercator merc = new Mercator(xIndex, yIndex, Window.LonResolution);
        Coord coord = merc.ToCoord(Window);

        return new WorldSample
        {
            Index = merc,
            Coord = coord,
            WorldPos = ConvertMapIndexToWorldPos(merc),
            Height = _HeightMap[merc.FlatSample] + _MountainMap[merc.FlatSample],
            Heat = _HeatMap[merc.FlatSample],
            Moisture = _MoistureMap[merc.FlatSample]
        };
    }

    public void UpdateViewWindow(ViewData window)
    {
        Window = window;
        OnWindowUpdated();
    }

    public ViewData ViewWindow()
    {
        return Window;
    }

    public float Height(int xIndex, int yIndex)
    {
        int index = GetMapIndex(xIndex, yIndex);
        return _HeightMap[index] + _MountainMap[index];
    }

    public float Heat(int xIndex, int yIndex)
    {
        return _HeatMap[GetMapIndex(xIndex, yIndex)];
    }

    public float Moisture(int xIndex, int yIndex)
    {
        return _MoistureMap[GetMapIndex(xIndex, yIndex)];
    }

    public float MinWorldHeight()
    {
        return NoiseValueToWorldHeight(_WorldHeightMin);
    }

    public float MaxWorldHeight()
    {
        return NoiseValueToWorldHeight(_WorldHeightMax);
    }

    public int MapIndexWidth()
    {
        return Window.LonResolution;
    }

    public int MapIndexHeight()
    {
        return Window.LatResolution;
    }


    /// <summary>
    /// Private Properties
    /// </summary>
    private float[] _HeightMap { get; set; }
    private float[] _MountainMap { get; set; }
    private float[] _HeatMap { get; set; }
    private float[] _MoistureMap { get; set; }

    private float _WorldHeightMax;
    private float _WorldHeightMin;
    private float _LocalHeightMax;
    private float _LocalHeightMin;

    private ViewData _OldWindow;

    private int GetMapIndex(int x, int y)
    {
        return x * Window.LatResolution + y;
    }

    private void SetWorldHeights(float worldMinHeight, float worldMaxHeight)
    {
        _WorldHeightMin = worldMinHeight;
        _WorldHeightMax = worldMaxHeight;
    }

    private void SetLocalHeights(float localMinHeight, float localMaxHeight)
    {
        _LocalHeightMin = localMinHeight;
        _LocalHeightMax = localMaxHeight;
    }

    private Vector3 ConvertMapIndexToWorldPos(Mercator merc)
    {
        //get value between 0 - 1. 0 being world min height. 1 being worldmax height
        float noiseHeight = (Height(merc.XSample, merc.YSample) - _WorldHeightMin) / (_WorldHeightMax - _WorldHeightMin);
        noiseHeight = Mathf.Max(noiseHeight, WorldData.OceanLevel);

        float minNoiseHeight = (_LocalHeightMin - _WorldHeightMin) / (_WorldHeightMax - _WorldHeightMin);
        minNoiseHeight = Mathf.Max(minNoiseHeight, WorldData.OceanLevel);

        //calculate mesh height so the lowest point in the local window is at 0
        float meshY = (noiseHeight - minNoiseHeight) * HeightScale;

        //calculate x mesh location so the center of the map is at 0
        float topLeftX = (MapIndexWidth() - 1) / -2f;
        float meshX = (merc.XSample + topLeftX) * MapScale;

        //calculate z mesh location so the center of the map is at 0
        float topLeftZ = (MapIndexHeight() - 1) / 2f;
        float meshZ = (topLeftZ - merc.YSample) * MapScale;

        return new Vector3(meshX, meshY, meshZ);
    }

    private float NoiseValueToWorldHeight(float value)
    {
        //get value between 0 - 1. 0 being world min height. 1 being worldmax height
        float noiseHeight = (value - _WorldHeightMin) / (_WorldHeightMax - _WorldHeightMin);
        noiseHeight = Mathf.Max(noiseHeight, WorldData.OceanLevel);

        float minNoiseHeight = (_LocalHeightMin - _WorldHeightMin) / (_WorldHeightMax - _WorldHeightMin);
        minNoiseHeight = Mathf.Max(minNoiseHeight, WorldData.OceanLevel);

        float meshHeight = (noiseHeight - minNoiseHeight) * HeightScale;

        return meshHeight;
    }

    private void InitializeMaxHeights()
    {
        ViewData fullWindow = new ViewData();
        fullWindow.ViewAngle = Mathf.PI * 2;
        fullWindow.Resolution = 128;

        float maxHeight, minHeight;
        Noise.GenerateNoiseMap(new List<NoiseData> { WorldData.HeightData, WorldData.MountainData }, fullWindow, WorldData.WorldRadius, out minHeight, out maxHeight);
        SetWorldHeights(minHeight, maxHeight);
    }

    private void UpdateMapArrays()
    {
        GenerateNoiseMaps();
        RefineWorldMaps();
        GenerateClimateMap();
    }

    private void GenerateNoiseMaps()
    {
        NoiseJobArray noiseJobs = new NoiseJobArray();
        noiseJobs.Add("height", WorldData.HeightData, Window, WorldData.WorldRadius);
        noiseJobs.Add("mountain", WorldData.MountainData, Window, WorldData.WorldRadius);
        noiseJobs.Add("heat", WorldData.HeatData, Window, WorldData.WorldRadius);
        noiseJobs.Add("moisture", WorldData.MoistureData, Window, WorldData.WorldRadius);

        noiseJobs.RunAll();

        float minHeight = noiseJobs.LocalMin("height") + noiseJobs.LocalMin("mountain");
        float maxHeight = noiseJobs.LocalMax("height") + noiseJobs.LocalMax("mountain");
        SetLocalHeights(minHeight, maxHeight);

        _HeightMap = noiseJobs.CopyNoise("height");
        _MountainMap = noiseJobs.CopyNoise("mountain");
        _HeatMap = noiseJobs.CopyNoise("heat");
        _MoistureMap = noiseJobs.CopyNoise("moisture");

        noiseJobs.Dispose();
    }

    private void RefineWorldMaps()
    {
        _MoistureMap = MoistureGen.RefineMap(_MoistureMap, _HeightMap, _MountainMap, WorldData, Window);
        //_MoistureMap = MoistureGen.TestWindAOE(_MoistureMap, _HeightMap, _MountainMap, WorldData, Window);
    }

    private void GenerateClimateMap()
    {

    }

    private void OnWorldDataUpdated()
    {
        InitializeMaxHeights();
        UpdateMapArrays();
        NotifyOfUpdatedValues();
    }

    private void OnWindowUpdated()
    {
        UpdateMapArrays();
        NotifyOfUpdatedValues();
    }

    private void OnValidate()
    {
        if (WorldData != null)
        {
            WorldData.OnValuesUpdated.RemoveListener(OnWorldDataUpdated);
            WorldData.OnValuesUpdated.AddListener(OnWorldDataUpdated);
        }

        if(Window != _OldWindow)
        {
            OnWindowUpdated();
            _OldWindow = Window;
        }
        base.OnValidate();
    }
}

[Serializable]
public struct WorldSample
{
    public Mercator Index;
    public Coord Coord;

    public Vector3 WorldPos;

    public float Height;
    public float Heat;
    public float Moisture;

    public static WorldSample Empty = new WorldSample
    {
        Index = new Mercator(-1, -1, 0)
    };

    public bool IsEmpty()
    {
        return Index.XSample == -1 && Index.XSample == -1;
    }
}
