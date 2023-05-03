using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public struct ViewData
{
    public static float MinViewAngle = 0.002f;

    [Range(-3.14159f, 3.14159f)]
    public float XRotation;

    [Range(-3.14159f, 3.14159f)]
    public float YRotation;

    [Range(-3.14159f, 3.14159f)]
    public float ZRotation;

    //total angle that the window spans window
    [Range(0f, 2 * 3.14159f)]
    public float ViewAngle;
    public float LonAngle { get { return ViewAngle; } }
    public float LatAngle { get { return ViewAngle/2; } }

    public int Resolution;
    public int LonResolution { get { return Resolution; } }
    public int LatResolution { get { return Resolution / 2; } }


    public override bool Equals(object o)
    {
        if (!(o is ViewData))
            return false;
        ViewData other = (ViewData)o;
        
        return this.XRotation == other.XRotation &&
            this.YRotation == other.YRotation &&
            this.ZRotation == other.ZRotation &&
            this.LonAngle == other.LonAngle &&
            this.LatAngle == other.LatAngle &&
            this.Resolution == other.Resolution;
    }

    public static bool operator==(ViewData lhs, ViewData rhs)
    {
        return lhs.Equals(rhs);            
    }

    public static bool operator !=(ViewData lhs, ViewData rhs)
    {
        return !lhs.Equals(rhs);
    }
}

