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
    public NoiseData windVelocityData;
    public NoiseData windRotationData;

    [Header("World Data")]
    public WorldSampler worldSampler;
    public TerrainData terrainData;
    public ViewData viewData;

    [HideInInspector]
    private bool MapInvalidated;
    [HideInInspector]
    private bool WindowUpdated;

    [HideInInspector]
    //private float WorldMinHeight = 0.0f;
    //[HideInInspector]
    //private float WorldMaxHeight = 0.0f;

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
        worldSampler.SetWorldHeights(minHeight, maxHeight);
    }

    public void GenerateMap()
    {
        if (!worldSampler.WorldHeightsSet())
            InitializeMap();

        //create map data points from noise 
        float maxHeight, minHeight;
        worldSampler.HeightMap = Noise.GenerateNoiseMap(new List<NoiseData> { heightData, mountainData }, viewData, terrainData, out minHeight, out maxHeight);
        worldSampler.SetLocalHeights(minHeight, maxHeight);
        worldSampler.HeatMap = Noise.GenerateNoiseMap(heatData, viewData, terrainData);
        worldSampler.WindVelocityMap = Noise.GenerateNoiseMap(windVelocityData, viewData, terrainData);
        worldSampler.WindRotationMap = Noise.GenerateNoiseMap(windRotationData, viewData, terrainData);
        //find max and min values of the mesh that is about to be created
        textureData.UpdateMeshHeights(terrainMaterial, worldSampler.MinMeshHeight(), worldSampler.MaxMeshHeight());

        //create mesh for the 2D mercator map
        MeshData meshData = MeshGenerator.GenerateTerrainMesh(worldSampler);
        meshFilterFlat.sharedMesh = meshData.CreateMesh();

        if(textureData.TextureType == TextureType.HeatMap)
        {
            //apply heat map texture to 2D mercator map
            Texture2D texture = TextureGenerator.GenerateHeatMapTexture(worldSampler.HeatMap, textureData, viewData);
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

        if (worldSampler != null)
        {
            worldSampler.OnValuesUpdated.RemoveListener(OnValuesUpdated);
            worldSampler.OnValuesUpdated.AddListener(OnValuesUpdated);
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
