using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Noise Attenuation/Parabola")]
public class ParabolaAttenuationData : NoiseAttenuationData
{
    public ActiveAxis ActiveAxis;

    public float A = 1.0f;
    public float B = 1.0f;
    public float C = 1.0f;

    public override float Evaluate(float xCoord, float yCoord)
    {
        float axisVal = 0.0f;
        if(ActiveAxis == ActiveAxis.X)
            axisVal = xCoord;
        else if (ActiveAxis == ActiveAxis.Y)
            axisVal = yCoord;


        float val = (A*axisVal*axisVal) + (B*axisVal) + C;
        if (val < 0.0f) val = 0.0f;
        if (val > 1.0f) val = 1.0f;
        return val;
    }
}

public enum ActiveAxis { X,Y,Z}
