using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapDisplay : MonoBehaviour
{
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    public ColorStyle DisplayVersion = ColorStyle.WaterLand;

    public void DrawMesh(float[,] noiseMap, MeshData meshData, float oceanLevel)
    {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);

        Texture2D texture = TextureGenerator.GenerateTexture(noiseMap, DisplayVersion, oceanLevel);

        meshFilter.sharedMesh = meshData.CreateMesh();
        meshRenderer.sharedMaterial.mainTexture = texture;

    }
}
