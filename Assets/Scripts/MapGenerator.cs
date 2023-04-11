using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MapGenerator : MonoBehaviour
{
    [Header ("Mesh Settings")]
    public MeshFilter meshFilterFlat;
    public MeshRenderer meshRendererFlat;

    public MeshFilter meshFilterSphere;
    public MeshRenderer meshRendererSphere;

    [Header("Display Settings")]
    public TextureData textureData;
    public Material terrainMaterial;
    public Material heatMaterial;

    [Header("Noise Data")]
    public NoiseData heightData;
    public NoiseData mountainData;
    public NoiseData heatData;

    [Header("World Data")]
    public TerrainData terrainData;
    public ViewData viewData;

    [HideInInspector]
    private bool MapInvalidated;
    [HideInInspector]
    private bool WindowUpdated;

    [HideInInspector]
    private float WorldMinHeight = 0.0f;
    [HideInInspector]
    private float WorldMaxHeight = 0.0f;

    public void start()
    {
        InitializeMap();
    }

    public void Update()
    {
        if(MapInvalidated)
        {
            InitializeMap();
            GenerateMap();
            MapInvalidated = false;
        }
        else if(WindowUpdated)
        {
            GenerateMap();
            WindowUpdated = false;
        }
    }

    public void InitializeMap()
    {
        ViewData fullWindow = new ViewData();
        fullWindow.LonAngle = Mathf.PI * 2;
        fullWindow.LatAngle = Mathf.PI;
        fullWindow.Resolution = 128;

        float maxHeight, minHeight;
        Noise.GenerateNoiseMap(new List<NoiseData> { heightData, mountainData }, fullWindow, terrainData, out minHeight, out maxHeight);
        WorldMinHeight = minHeight;
        WorldMaxHeight = maxHeight;

        //create wind currents here
    }

    public void GenerateMap()
    {
        if (WorldMaxHeight == 0.0f && WorldMinHeight == 0.0f)
            InitializeMap();

        //create map data points from noise 
        float maxHeight, minHeight;
        float[,] heightMap = Noise.GenerateNoiseMap(new List<NoiseData> { heightData, mountainData }, viewData, terrainData, out minHeight, out maxHeight);
        float[,] heatMap = Noise.GenerateNoiseMap(heatData, viewData, terrainData);

        //find max and min values of the mesh that is about to be created
        float maxMeshHeight = MeshGenerator.ConvertNoiseValueToMeshHeight(WorldMaxHeight, terrainData, WorldMinHeight, WorldMaxHeight, minHeight);
        float minMeshHeight = MeshGenerator.ConvertNoiseValueToMeshHeight(WorldMinHeight, terrainData, WorldMinHeight, WorldMaxHeight, minHeight);
        textureData.UpdateMeshHeights(terrainMaterial, minMeshHeight, maxMeshHeight);

        //create mesh for the 2D mercator map
        MeshData meshData = MeshGenerator.GenerateTerrainMesh(heightMap, viewData, terrainData, WorldMinHeight, WorldMaxHeight, minHeight);
        meshFilterFlat.sharedMesh = meshData.CreateMesh();

        if(textureData.TextureType == TextureType.HeatMap)
        {
            //apply heat map texture to 2D mercator map
            Texture2D texture = TextureGenerator.GenerateHeatMapTexture(heatMap, textureData, viewData);
            heatMaterial.SetTexture("_MainTex", texture);
            meshRendererFlat.sharedMaterial = heatMaterial;
        }
        else if(textureData.TextureType == TextureType.HeightMap)
        {
            meshRendererFlat.sharedMaterial = terrainMaterial;
        }

        //create spherical map 
        MeshData meshDataSphere = MeshGenerator.GenerateSphereMesh(viewData);
        meshFilterSphere.sharedMesh = meshDataSphere.CreateMesh();
    }

    private void OnTextureValuesUpdated()
    {
        textureData.ApplyToMaterial(terrainMaterial);
        if (Application.isPlaying)
            WindowUpdated = true;
        else
            GenerateMap();
    }

    private void OnWindowUpdated()
    {
        if (Application.isPlaying)
            WindowUpdated = true;
        else
            GenerateMap();
    }

    private void OnValuesUpdated()
    {
        if(Application.isPlaying)
            MapInvalidated = true;
        else
        {
            InitializeMap();
            GenerateMap();
        }
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

        if (mountainData != null)
        {
            mountainData.OnValuesUpdated.RemoveListener(OnValuesUpdated);
            mountainData.OnValuesUpdated.AddListener(OnValuesUpdated);
        }

        if (terrainData != null)
        {
            terrainData.OnValuesUpdated.RemoveListener(OnValuesUpdated);
            terrainData.OnValuesUpdated.AddListener(OnValuesUpdated);
        }

        if (viewData != null)
        {
            viewData.OnValuesUpdated.RemoveListener(OnWindowUpdated);
            viewData.OnValuesUpdated.AddListener(OnWindowUpdated);
        }

        if (textureData != null)
        {
            textureData.OnValuesUpdated.RemoveListener(OnTextureValuesUpdated);
            textureData.OnValuesUpdated.AddListener(OnTextureValuesUpdated);
        }
    }
}
