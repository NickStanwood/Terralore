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
        float heightMultiplier = (ViewData.MaxLon * terrain.HeightScale) / window.LonAngle;
        MeshData meshData = new MeshData(width, height);
        int vertexIndex = 0;

        //check if part of the mesh is going to be ocean
        if (terrain.OceanLevel > localMin)
            localMin = terrain.OceanLevel;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float heightY = Mathf.Max(HeightMap[x, y] - localMin, 0.0f);

                heightY *= heightMultiplier;

                meshData.Vertices[vertexIndex] = new Vector3(topLeftX + x, heightY, topLeftZ - y);
                meshData.UVs[vertexIndex] = new Vector2(x / (float)width, y / (float)height);

                if (x < width - 1 && y < height - 1)
                {
                    meshData.AddTriangle(vertexIndex, vertexIndex + width + 1, vertexIndex + width);
                    meshData.AddTriangle(vertexIndex + width + 1, vertexIndex, vertexIndex + 1);
                }

                vertexIndex++;
            }
        }

        return meshData;
    }

    public static MeshData GenerateSphereMesh(ViewData window)
    {
        float radius = 100.0f;
        int width = window.LonResolution;
        int height = window.LatResolution;

        MeshData meshData = new MeshData(width, height);
        int vertexIndex = 0;

        float lonSampleFreq = window.LonAngle / window.LonResolution;
        float latSampleFreq = window.LatAngle / window.LatResolution;

        for (int lat = 0; lat < height; lat++)
        {
            for (int lon = 0; lon < width; lon++)
            {
                float sampleLon = lon * lonSampleFreq - ViewData.MinLon;
                float sampleLat = lat * latSampleFreq - ViewData.MinLat;

                float x = radius * Mathf.Cos(sampleLat) * Mathf.Cos(sampleLon);
                float y = radius * Mathf.Cos(sampleLat) * Mathf.Sin(sampleLon);
                float z = radius * Mathf.Sin(sampleLat);

                meshData.Vertices[vertexIndex] = new Vector3(x, y, z);
                meshData.UVs[vertexIndex] = new Vector2((float)lon / width, (float)lat / height);

                if (lat < height - 1 && lon < width - 1)
                {
                    meshData.AddTriangle(vertexIndex, vertexIndex + width, vertexIndex + width + 1);
                    meshData.AddTriangle(vertexIndex + width + 1, vertexIndex + 1, vertexIndex);
                }
                else if (lon == width - 1 && lat < height - 1 && lat > 0)
                {
                    meshData.AddTriangle(vertexIndex, vertexIndex + width, vertexIndex + 1);
                    meshData.AddTriangle(vertexIndex + 1, vertexIndex - width, vertexIndex);
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
        int triangleCount = (width * (height - 1) - 1) * 6;
        Debug.Log($"triangleCount:{triangleCount}");
        Triangles = new int[triangleCount];
    }

    public void AddTriangle(int a, int b, int c)
    {
        if (a > 128 || b > 128 || c > 128)
            Debug.Log($"a:{a}, b:{b}, c:{c}");

        Triangles[triangleIndex] = a;
        Triangles[triangleIndex + 1] = b;
        Triangles[triangleIndex + 2] = c;
        triangleIndex += 3;
        Debug.Log($"triangleIndex:{triangleIndex}");
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
