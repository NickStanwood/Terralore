using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoordDebugger : MonoBehaviour
{
    public WorldSampler WorldSampler;

    [Header("Sample From coord")]
    [Range(-3.14f, 3.14f)]
    public float Lon = 0.0f;

    [Range(-3.14f / 2, 3.14f / 2)]
    public float Lat = 0.0f;

    public WorldSample CoordSample;
    public Transform CoordSphere;

    [Header("Sample From coord")]
    [Range(0, 256)]
    public int xIndex = 0;
    [Range(0, 128)]
    public int yIndex = 0;
    public WorldSample IndexSample;
    public Transform IndexSphere;


    public void OnValidate()
    {
        CoordSample = WorldSampler.SampleFromCoord(Lon, Lat);
        CoordSphere.position = CoordSample.WorldPos;
        IndexSample = WorldSampler.SampleFromIndex(xIndex, yIndex);
        IndexSphere.position = IndexSample.WorldPos;
    }

}
