using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class TerrainData : UpdatableData
{
    [Range(500f, 10000f)]
    public float WorldRadius;

    [Range(0.0f, 1.0f)]
    public float OceanLevel;

    [Range(0.0f, 1000.0f)]
    public float HeightScale;
}
