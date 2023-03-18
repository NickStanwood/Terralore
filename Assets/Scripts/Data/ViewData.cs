using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class ViewData : UpdatableData
{
    #region static properties
    public static float MaxLon = Mathf.PI;
    public static float MaxLat = Mathf.PI/2;


    public static float MinLon = -Mathf.PI;
    public static float MinLat = -Mathf.PI/2;
    #endregion

    //Top left corner
    [Range(-3.14159f, 3.14159f)]
    public float LonLeft;

    [Range(-3.14159f/2f, 3.14159f/2f)]
    public float LatTop;

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
            LonAngle = Mathf.Min(LonAngle, MaxLon - MinLon);
            LatAngle = LonAngle / 2f;
        }
        else if(_OldLatAngle != LatAngle)
        {
            LatAngle = Mathf.Min(LatAngle, MaxLat-MinLat);
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
