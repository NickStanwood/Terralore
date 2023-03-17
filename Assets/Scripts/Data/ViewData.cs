using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class ViewData : UpdatableData
{
    #region static properties
    public static float MaxLon = 2 * Mathf.PI;
    public static float MaxLat = Mathf.PI;
    #endregion

    //Top left corner
    public float LonLeft;     
    public float LatTop;

    //total angle that the window spans window
    public float LonAngle;
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

    public float LonRight()
    {
        return LonLeft + LonAngle;
    }

    public float LatBottom()
    {
        return LatTop + LatAngle;
    }

    protected override void OnValidate()
    {
        if(_OldLonAngle != LonAngle)
        {
            LonAngle = Mathf.Min(LonAngle, MaxLon);
            LatAngle = LonAngle / 2f;
        }
        else if(_OldLatAngle != LatAngle)
        {
            LatAngle = Mathf.Min(LatAngle, MaxLat);
            LonAngle = LatAngle*2f;
        }
            

        if(float.IsNaN(LonAngle)      || float.IsNaN(LatAngle)      ||
           float.IsInfinity(LonAngle) || float.IsInfinity(LatAngle) ||
           LonAngle < MinAngle || LatAngle < MinAngle / 2f)
        {
            LonAngle = MinAngle;
            LatAngle = MinAngle/2f;
        }

        if(LonAngle > MaxLon || LatAngle > MaxLat)
        {
            LonAngle = MaxLon;
            LatAngle = MaxLat;
        }

        Resolution = (Resolution < 16) ? 16 : Resolution;

        _OldLonAngle = LonAngle;
        _OldLatAngle = LatAngle;
        _OldResolution = Resolution;       

        base.OnValidate();
    }
}
