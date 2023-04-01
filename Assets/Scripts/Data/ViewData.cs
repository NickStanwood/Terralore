using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class ViewData : UpdatableData
{

    [Range(-3.14159f, 3.14159f)]
    public float XRotation;

    [Range(-3.14159f, 3.14159f)]
    public float YRotation;

    [Range(-3.14159f, 3.14159f)]
    public float ZRotation;

    //total angle that the window spans window
    [Range(0f, 2 * 3.14159f)]
    public float LonAngle;

    [Range(0f, 3.14159f)]
    public float LatAngle;

    public float MinAngle = 0.002f;


    //how many mesh nodes per edge
    public int Resolution = 256;
    public int LonResolution { get { return Resolution; } }
    public int LatResolution { get { return Resolution/2; } }

    [HideInInspector]
    private float _OldLonAngle;
    [HideInInspector]
    private float _OldLatAngle;

    [HideInInspector]
    private float _OldResolution;

    protected override void OnValidate()
    {
        if(_OldLonAngle != LonAngle)
        {
            LonAngle = Mathf.Min(LonAngle, Coordinates.MaxLon - Coordinates.MinLon);
            LatAngle = LonAngle / 2f;
        }
        else if(_OldLatAngle != LatAngle)
        {
            LatAngle = Mathf.Min(LatAngle, Coordinates.MaxLat - Coordinates.MinLat);
            LonAngle = LatAngle*2f;
        }
            

        if(float.IsNaN(LonAngle)      || float.IsNaN(LatAngle)      ||
           float.IsInfinity(LonAngle) || float.IsInfinity(LatAngle) ||
           LonAngle < MinAngle || LatAngle < MinAngle / 2f)
        {
            LonAngle = MinAngle;
            LatAngle = MinAngle/2f;
        }

        Resolution = (Resolution < 16) ? 16 : Resolution;

        _OldLonAngle = LonAngle;
        _OldLatAngle = LatAngle;
        _OldResolution = Resolution;       

        base.OnValidate();
    }
}
