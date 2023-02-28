using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapDisplay : MonoBehaviour
{
    public Renderer textureRenderer;

    public Color LandColor;
    public Color SeaColor;

    [Range(0.0f, 1.0f)]
    public float WaterLevel;

    public bool UseGreyScale = false;

    public void DrawNoiseMap(float[,] noiseMap)
    {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);

        Texture2D texture = new Texture2D(width, height);

        Color[] colorMap = new Color[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if(UseGreyScale)
                    colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, noiseMap[x, y]);
                else if (noiseMap[x, y] > WaterLevel)
                    colorMap[y * width + x] = LandColor;
                else
                    colorMap[y * width + x] = SeaColor;                
            }
        }

        texture.SetPixels(colorMap);
        texture.Apply();

        textureRenderer.sharedMaterial.mainTexture = texture;
        textureRenderer.transform.localScale = new Vector3(width, 1, height);
    }
}
