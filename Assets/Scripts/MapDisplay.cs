using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapDisplay : MonoBehaviour
{
    public Renderer textureRenderer;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    public ColorStyle DisplayVersion = ColorStyle.WaterLand;

    public void DrawNoiseMap(float[,] noiseMap, float oceanLevel)
    {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);

        Texture2D texture = TextureGenerator.GenerateTexture(noiseMap, DisplayVersion, oceanLevel);

        textureRenderer.sharedMaterial.mainTexture = texture;
        textureRenderer.transform.localScale = new Vector3(width, 1, height);
    }

    public void DrawMesh(float[,] noiseMap, MeshData meshData, float oceanLevel)
    {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);

        Texture2D texture = TextureGenerator.GenerateTexture(noiseMap, DisplayVersion, oceanLevel);

        meshFilter.sharedMesh = meshData.CreateMesh();
        meshRenderer.sharedMaterial.mainTexture = texture;

    }
}
