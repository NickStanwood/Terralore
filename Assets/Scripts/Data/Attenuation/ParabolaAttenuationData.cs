//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//[CreateAssetMenu(menuName = "Noise Attenuation/Parabola")]
//public class ParabolaAttenuationData : NoiseAttenuationData
//{
//    [Range(0.0f, 1.0f)]
//    public float EquatorAttenuation;
//    [Range(0.0f, 1.0f)]
//    public float PoleAttenuation;

//    public override float Evaluate(float x, float y, float z)
//    {
//        float r2 = x*x + y*y + z*z;
//        float lat2 = y * y;
//        float val = (PoleAttenuation - EquatorAttenuation)*lat2/r2 + EquatorAttenuation;
//        if (val < 0.0f) val = 0.0f;
//        if (val > 1.0f) val = 1.0f;
//        return val;
//    }
//}
