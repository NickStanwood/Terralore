using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureGenerator
{
    public static Texture2D GenerateTexture(float[,] noiseMap, DisplayData display, float waterLevel, ViewData window)
    {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);

        Texture2D texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Point;
        //texture.wrapMode = TextureWrapMode.Clamp;

        Color[] colorMap = new Color[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                switch (display.Style)
                {
                    case ColorStyle.GreyScale:
                        colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, noiseMap[x, y]);
                        break;
                    case ColorStyle.HeightMap:
                        if (noiseMap[x, y] < waterLevel)
                        {
                            colorMap[y * width + x] = display.WaterColor;
                            continue;
                        }

                        foreach (var range in display.HeightMapColors)
                        {
                            if (noiseMap[x, y] < range.MaxValue)
                            {
                                colorMap[y * width + x] = range.Color;
                                break;
                            }
                        }
                        break;
                    default:
                        if (noiseMap[x, y] > waterLevel)
                            colorMap[y * width + x] = Color.white;
                        else
                            colorMap[y * width + x] = Color.black;
                        break;
                }

                Vector2 coord = Coordinates.MercatorToCoord((float)x / width, (float)y / height, window);
                foreach(Vector2 poi in window.PointsOfInterest)
                {
                    if (InRange(coord.x, poi.x) && InRange(coord.y, poi.y))
                    {
                        colorMap[y * width + x] = Color.red;
                    }

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
