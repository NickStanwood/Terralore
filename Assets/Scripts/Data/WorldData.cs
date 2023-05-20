using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class WorldData : UpdatableData
{
    [Header("Noise Data")]
    public NoiseData HeightData;
    public NoiseData MountainData;
    public NoiseData HeatData;
    public NoiseData MoistureData;

    [Header("World Data")]
    [Range(0.0f, 1.0f)]
    public float OceanLevel;
    [Range(0.0f, 1.0f)]
    public float WindVelocity;
    [Range(500f, 10000f)]
    public float WorldRadius;
}
