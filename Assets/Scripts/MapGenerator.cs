using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MapGenerator : MonoBehaviour
{

    public MeshFilter meshFilterFlat;
    public MeshRenderer meshRendererFlat;

    public MeshFilter meshFilterSphere;
    public MeshRenderer meshRendererSphere;

    public NoiseData heightData;
    public NoiseData heatData;
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
        float[,] heightMap = Noise.GenerateNoiseMap(heightData, viewData, terrainData, out minHeight, out maxHeight);
        float[,] heatMap = Noise.GenerateNoiseMap(heatData, viewData, terrainData);
        MeshData meshData = MeshGenerator.GenerateTerrainMesh(heightMap, viewData, terrainData, minHeight);

        Texture2D texture = TextureGenerator.GenerateTexture(heightMap, heatMap, displayData, terrainData.OceanLevel, viewData);

        meshFilterFlat.sharedMesh = meshData.CreateMesh();
        meshRendererFlat.sharedMaterial.mainTexture = texture;


        MeshData meshDataSphere = MeshGenerator.GenerateSphereMesh(viewData);
        meshFilterSphere.sharedMesh = meshDataSphere.CreateMesh();
        meshRendererSphere.sharedMaterial.mainTexture = texture;
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

        if(heightData != null)
        {
            heightData.OnValuesUpdated.RemoveListener(OnValuesUpdated);
            heightData.OnValuesUpdated.AddListener(OnValuesUpdated);
        }

        if (heatData != null)
        {
            heatData.OnValuesUpdated.RemoveListener(OnValuesUpdated);
            heatData.OnValuesUpdated.AddListener(OnValuesUpdated);
        }

        if (terrainData != null)
        {
            terrainData.OnValuesUpdated.RemoveListener(OnValuesUpdated);
            terrainData.OnValuesUpdated.AddListener(OnValuesUpdated);
        }

        if (viewData != null)
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
