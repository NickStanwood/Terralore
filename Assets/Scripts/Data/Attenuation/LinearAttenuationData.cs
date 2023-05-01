//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//[CreateAssetMenu(menuName = "Noise Attenuation/Linear")]
//public class LinearAttenuationData : NoiseAttenuationData
//{
//    public float XSlope = 1.0f;
//    public float YSlope = 1.0f;
//    public float offset = 0.5f;

//    public override float Evaluate(float xCoord, float yCoord)
//    {
//        float val = xCoord * XSlope + yCoord * YSlope + offset;
//        if (val < 0.0f) val = 0.0f;
//        if (val > 1.0f) val = 1.0f;
//        return val;
//    }
//}
