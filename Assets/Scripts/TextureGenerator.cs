using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureGenerator
{
    public static Texture2D GenerateHeatMapTexture(float[,] heatMap, TextureData data, ViewData window)
    {
        int width = heatMap.GetLength(0);
        int height = heatMap.GetLength(1);

        Texture2D texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Point;

        Color[] colorMap = new Color[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                foreach (var range in data.HeatLayers)
                {
                    float drawStrength = Mathf.InverseLerp(-range.BlendStrength / 2-0.0001f, range.BlendStrength / 2, heatMap[x, y] - range.StartHeight);
                    colorMap[y * width + x] = colorMap[y * width + x] * (1 - drawStrength) + range.Colour * drawStrength;
                }
            }
        }

        texture.SetPixels(colorMap);
        texture.Apply();

        return texture;
    }

    private static bool InRange(float a, float b)
    {
        if (a + 0.01 > b && a - 0.01 < b)
            return true;
        return false;
    }
}
