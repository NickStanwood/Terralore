//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//[CreateAssetMenu(menuName = "Noise Attenuation/Tan")]
//public class TanAttenuationData : NoiseAttenuationData
//{
//    [Range(0.0f, 1.0f)]
//    public float Amplitude;

//    [Range(0.0f, 10.0f)]
//    public float Frequency;

//    [Range(0.0f, 1)]
//    public float Offset;

//    [Range(0.1f, 0.25f)]
//    public float AsymptoteCutoff;

//    public override float Evaluate(float x, float y, float z)
//    {
//        float scale = Mathf.Tan(Mathf.PI / 2 + AsymptoteCutoff);
//        float r = Mathf.Sqrt(x * x + y * y + z * z);
//        float lat = (y * Mathf.PI) / r;
        
//        //check if value is inside the range of the asymptote
//        float val = (lat * Frequency) + Offset * Mathf.PI * 2;
//        float abs = Mathf.Abs(val);
//        if (abs < Mathf.PI / 2 + AsymptoteCutoff && abs > Mathf.PI / 2 - AsymptoteCutoff)
//            return 0.5f*Amplitude;

//        val = Mathf.Tan(val) * Amplitude/2;
//        val = (val / scale) + 0.5f;

//        return val;
//    }
//}
