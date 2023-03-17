using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MapGenerator : MonoBehaviour
{

    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    public NoiseData noiseData;
    public TerrainData terrainData;
    public ViewData viewData;
    public DisplayData displayData;

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
        float[,] map = Noise.GenerateNoiseMap(noiseData, viewData, terrainData, out minHeight, out maxHeight);
        //MeshData meshData = MeshGenerator.GenerateTerrainMesh(map, viewData, terrainData, minHeight);
        MeshData meshData = MeshGenerator.GenerateSphereMesh(viewData, terrainData);

        Texture2D texture = TextureGenerator.GenerateTexture(map, displayData, terrainData.OceanLevel);

        meshFilter.sharedMesh = meshData.CreateMesh();
        meshRenderer.sharedMaterial.mainTexture = texture;
    }

    public void OnValuesUpdated()
    {
        if(Application.isPlaying)
            MapInvalidated = true;
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

        if (displayData != null)
        {
            displayData.OnValuesUpdated.RemoveListener(OnValuesUpdated);
            displayData.OnValuesUpdated.AddListener(OnValuesUpdated);
        }
    }
}
