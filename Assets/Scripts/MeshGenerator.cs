using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
    public static MeshData GenerateTerrainMesh(WorldSampler worldSampler)
    {
        int width = worldSampler.HeightMap.GetLength(0);
        int height = worldSampler.HeightMap.GetLength(1);

        MeshData meshData = new MeshData(width, height);
        int vertexIndex = 0;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3 pos = worldSampler.SampleFromIndex(x, y).WorldPos;
                meshData.Vertices[vertexIndex] = pos;
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

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float xPercent = (float)x / window.LonResolution;
                float yPercent = (float)y / window.LatResolution;
                Vector3 c = Coordinates.MercatorToCartesian(xPercent, yPercent, window, radius);

                meshData.Vertices[vertexIndex] = new Vector3(c.x, c.y, c.z);
                meshData.UVs[vertexIndex] = new Vector2((float)x / width, (float)y / height);

                if (y < height - 1 && x < width - 1)
                {
                    meshData.AddTriangle(vertexIndex, vertexIndex + width, vertexIndex + width + 1);
                    meshData.AddTriangle(vertexIndex + width + 1, vertexIndex + 1, vertexIndex);

                    //meshData.AddTriangle(vertexIndex, vertexIndex + width + 1, vertexIndex + width);
                    //meshData.AddTriangle(vertexIndex + width + 1, vertexIndex, vertexIndex + 1);
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
        int triangleCount = (width - 1) * (height - 1) * 6;
        Triangles = new int[triangleCount];
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
