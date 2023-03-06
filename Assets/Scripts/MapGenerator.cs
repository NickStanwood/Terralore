using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MapGenerator : MonoBehaviour
{
    public NoiseData noiseData;
    public TerrainData terrainData;
    public ViewData viewData;

    [HideInInspector]
    private bool MapInvalidated;

    public void start()
    {
    }

    public void Update()
    {
        if(MapInvalidated)
        {
            GenerateMap();
            MapInvalidated = false;
        }
    }

    public void GenerateMap()
    {
        float maxHeight, minHeight;
        float[,] map = Noise.GenerateNoiseMap(noiseData, viewData, out minHeight, out maxHeight);
        MeshData meshData = MeshGenerator.GenerateTerrainMesh(map, viewData, terrainData, minHeight);

        MapDisplay display = FindObjectOfType<MapDisplay>();
        display.DrawMesh(map, meshData, terrainData);
    }

    public void OnValuesUpdated()
    {
        if(Application.isPlaying)
            MapInvalidated = false;
        else
            GenerateMap();
    }

    private void OnValidate()
    {
        if(terrainData != null)
        {
            terrainData.OnValuesUpdated.RemoveListener(OnValuesUpdated);
            terrainData.OnValuesUpdated.AddListener(OnValuesUpdated);
        }

        if(noiseData != null)
        {
            noiseData.OnValuesUpdated.RemoveListener(OnValuesUpdated);
            noiseData.OnValuesUpdated.AddListener(OnValuesUpdated);
        }

        if(viewData != null)
        {
            viewData.OnValuesUpdated.RemoveListener(OnValuesUpdated);
            viewData.OnValuesUpdated.AddListener(OnValuesUpdated);
        }
    }
}
