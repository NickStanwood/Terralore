using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
    public static MeshData GenerateTerrainMesh(float[,] HeightMap, ViewData window, TerrainData terrain, float localMin)
    {
        int width = HeightMap.GetLength(0);
        int height = HeightMap.GetLength(1);

        float topLeftX = (width - 1) / -2f;
        float topLeftZ = (height - 1) / 2f;
        float heightMultiplier = (window.MaxWidth*terrain.HeightScale) / window.Width;
        Debug.Log("height multiplier: " + heightMultiplier);
        MeshData meshData = new MeshData(width, height);
        int vertexIndex = 0;

        //check if part of the mesh is going to be ocean
        if(terrain.OceanLevel > localMin)
            localMin = terrain.OceanLevel;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width ; x++)
            {
                float heightY = Mathf.Max(HeightMap[x, y] - localMin, 0.0f);

                heightY *= heightMultiplier;

                meshData.Vertices[vertexIndex] = new Vector3(topLeftX + x, heightY, topLeftZ - y);
                meshData.UVs[vertexIndex] = new Vector2(x / (float)width, y / (float)height);

                if(x < width - 1 && y < height - 1)
                {
                    meshData.AddTriangle(vertexIndex            , vertexIndex + width + 1, vertexIndex + width);
                    meshData.AddTriangle(vertexIndex + width + 1, vertexIndex            , vertexIndex + 1);
                }

                vertexIndex++;
            }
        }

        return meshData;
    }
}

public class MeshData
{
    public Vector3[] Vertices;
    public int[] Triangles;
    public Vector2[] UVs;

    private int triangleIndex = 0;

    public MeshData(int width, int height)
    {
        Vertices = new Vector3[width * height];
        UVs = new Vector2[width * height];
        Triangles = new int[(width - 1) * (height - 1) * 6];
    }

    public void AddTriangle(int a, int b, int c)
    {
        Triangles[triangleIndex] = a;
        Triangles[triangleIndex + 1] = b;
        Triangles[triangleIndex + 2] = c;
        triangleIndex += 3;
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = Vertices;
        mesh.triangles = Triangles;
        mesh.uv = UVs;
        mesh.RecalculateNormals();
        return mesh;
    }
}
