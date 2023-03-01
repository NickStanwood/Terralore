using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public int Seed;
    public NoiseParams noise;
    public ViewWindow viewWindow;
    public bool AutoUpdate;

    [Range(0.0f, 1.0f)]
    public float OceanLevel;

    public void GenerateMap()
    {
        float maxHeight, minHeight;
        float[,] map = Noise.GenerateNoiseMap(Seed, noise, viewWindow, out minHeight, out maxHeight);
        MeshData meshData = MeshGenerator.GenerateTerrainMesh(map, viewWindow, OceanLevel);

        MapDisplay display = FindObjectOfType<MapDisplay>();
        //display.DrawNoiseMap(map);
        display.DrawMesh(map, meshData, OceanLevel);
    }

    public void OnValidate()
    {
        viewWindow.OnValidate();

    }
}
