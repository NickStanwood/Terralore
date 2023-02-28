using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public int Seed;
    public NoiseParams noise;
    public ViewWindow viewWindow;
    public bool AutoUpdate;

    public void GenerateMap()
    {
        float[,] map = Noise.GenerateNoiseMap(Seed, noise, viewWindow);

        MapDisplay display = FindObjectOfType<MapDisplay>();
        display.DrawNoiseMap(map);
    }

    public void OnValidate()
    {
        viewWindow.OnValidate();

    }
}
