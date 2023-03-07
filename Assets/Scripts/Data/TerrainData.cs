using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class TerrainData : UpdatableData
{
    [Range(0.0f, 1.0f)]
    public float OceanLevel;

    [Range(0.0f, 25.0f)]
    public float HeightScale;
}
