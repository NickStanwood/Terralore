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
    public Material moistureMaterial;
    public Material blankMaterial;

    [Header("World Data")]
    public WorldSampler worldSampler;

    [HideInInspector]
    private bool MapInvalidated;
    [HideInInspector]
    private bool WindowUpdated;

    public void start()
    {
        textureData.ApplyToMaterial(terrainMaterial);
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
        //find max and min values of the mesh that is about to be created
        textureData.UpdateMeshHeights(terrainMaterial, worldSampler.MinWorldHeight(), worldSampler.MaxWorldHeight());

        //create mesh for the 2D mercator map
        MeshData meshData = MeshGenerator.GenerateTerrainMesh(worldSampler);
        meshFilterFlat.sharedMesh = meshData.CreateMesh();

        switch(textureData.TextureType)
        {
            case TextureType.HeightMap:
                meshRendererFlat.sharedMaterial = terrainMaterial;
                break;
            case TextureType.HeatMap:
                {
                    //apply heat map texture to 2D mercator map
                    Texture2D texture = TextureGenerator.GenerateHeatMapTexture(worldSampler, textureData);
                    heatMaterial.SetTexture("_MainTex", texture);
                    meshRendererFlat.sharedMaterial = heatMaterial;
                    break;
                }                
            case TextureType.MoistureMap:
                {
                    //apply heat map texture to 2D mercator map
                    Texture2D texture = TextureGenerator.GenerateMoistureMapTexture(worldSampler, textureData);
                    moistureMaterial.SetTexture("_MainTex", texture);
                    meshRendererFlat.sharedMaterial = moistureMaterial;
                    break;
                }                
            default:
                meshRendererFlat.sharedMaterial = blankMaterial;
                break;
        }

        //create spherical map 
        MeshData meshDataSphere = MeshGenerator.GenerateSphereMesh(worldSampler.ViewWindow());
        meshFilterSphere.sharedMesh = meshDataSphere.CreateMesh();
    }

    private void OnTextureValuesUpdated()
    {
        textureData.ApplyToMaterial(terrainMaterial);
        OnValuesUpdated();
    }

    private void OnValuesUpdated()
    {
        if (Application.isPlaying)
            MapInvalidated = true;
        else
        {
            textureData.ApplyToMaterial(terrainMaterial);
            GenerateMap();
        }
    }

    private void OnValidate()
    {
        if (worldSampler != null)
        {
            worldSampler.OnValuesUpdated.RemoveListener(OnValuesUpdated);
            worldSampler.OnValuesUpdated.AddListener(OnValuesUpdated);
        }

        if (textureData != null)
        {
            textureData.OnValuesUpdated.RemoveListener(OnTextureValuesUpdated);
            textureData.OnValuesUpdated.AddListener(OnTextureValuesUpdated);
        }
    }
}
