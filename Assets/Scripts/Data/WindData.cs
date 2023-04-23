using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class WindData : UpdatableData
{
    public bool ShowWindCurrents;

    public Color StartColour;
    public Color EndColour;

    [Range(0.0f,1.0f)]
    public float MovementSpeed;
}
