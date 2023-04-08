using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu()]
public class TextureData : UpdatableData
{
    public TextureLayer[] Layers;

    public void ApplyToMaterial(Material material)
    {
        material.SetInt("baseColourCount", Layers.Length);
        material.SetColorArray("baseColours", Layers.Select(l => l.Colour).ToArray());
        material.SetFloatArray("baseStartHeights", Layers.Select(l => l.StartHeight).ToArray());
        material.SetFloatArray("baseBlendStrength", Layers.Select(l => l.BlendStrength).ToArray());
    }

    public void UpdateMeshHeights(Material material, float minHeight, float maxHeight)
    {
        material.SetFloat("minHeight", minHeight);
        material.SetFloat("maxHeight", maxHeight);
    }
}

[System.Serializable]
public class TextureLayer
{
    public string Name;
    [Range(0.0f, 1.0f)]
    public float StartHeight;
    public Color Colour;
    [Range(0.0f, 1.0f)]
    public float BlendStrength;

}
