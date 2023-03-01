using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureGenerator
{
    public static Color LandColor = new Color(24/255f, 149/255f, 52/255f);
    public static Color WaterColor = new Color(57/255f, 140/255f, 204/255f);

    public static Texture2D GenerateTexture(float[,] noiseMap, ColorStyle style, float waterLevel)
    {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);

        Texture2D texture = new Texture2D(width, height);

        Color[] colorMap = new Color[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                switch (style)
                {
                    case ColorStyle.GreyScale:
                        colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, noiseMap[x, y]);
                        break;
                    default:
                        if (noiseMap[x, y] > waterLevel)
                            colorMap[y * width + x] = LandColor;
                        else
                            colorMap[y * width + x] = WaterColor;
                        break;
                }
            }
        }

        texture.SetPixels(colorMap);
        texture.Apply();

        return texture;
    }
}

public enum ColorStyle
{
    GreyScale,
    WaterLand,
    HeightMap,
    HeatMap,
    Mountains,
    Vintage
}
