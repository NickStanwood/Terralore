using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MapGenerator : MonoBehaviour
{
    public int Seed;
    public NoiseParams noise;

    public bool AutoUpdate;

    [Range(0.0f, 1.0f)]
    public float OceanLevel;

    [HideInInspector]
    private ViewWindow viewWindow;

    [HideInInspector]
    private bool MapInvalidated;

    public void start()
    {
    }

    public void Update()
    {
        if(MapInvalidated)
        {
            GenerateMap(viewWindow);
            MapInvalidated = false;
        }
    }

    public void GenerateMap(ViewWindow window)
    {
        float maxHeight, minHeight;
        float[,] map = Noise.GenerateNoiseMap(Seed, noise, window, out minHeight, out maxHeight);
        MeshData meshData = MeshGenerator.GenerateTerrainMesh(map, window, OceanLevel);

        MapDisplay display = FindObjectOfType<MapDisplay>();
        //display.DrawNoiseMap(map);
        display.DrawMesh(map, meshData, OceanLevel);
    }

    public void OnWindowUpdate(ViewWindow window)
    {
        MapInvalidated = true;
        viewWindow = window;
    }
}
