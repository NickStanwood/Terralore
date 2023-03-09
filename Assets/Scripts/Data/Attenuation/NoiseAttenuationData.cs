using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Noise Attenuation/Constant")]
public class NoiseAttenuationData : UpdatableData
{
    public virtual float Evaluate(float xCoord, float yCoord)
    {
        return 1.0f;
    }
}