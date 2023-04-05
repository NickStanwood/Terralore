using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class DisplayData : UpdatableData
{
    public ColorStyle Style = ColorStyle.HeightMap;
    #region Height Map
    public Color WaterColor;
    public List<DisplayRange> HeightMapColors;
    #endregion
    #region Heat Map
    public Color LandColor;
    public List<DisplayRange> HeatMapColors;
    #endregion
}

[System.Serializable]
public class DisplayRange
{
    public string Name;
    [Range(0.0f, 1.0f)]
    public float MaxValue;
    public Color Color;
}


public enum ColorStyle
{
    GreyScale,
    HeightMap,
    HeatMap,
    Mountains,
    Vintage
}