using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Noise Attenuation/Ridged")]
public class RidgedAttenuationData : NoiseAttenuationData
{
    public NoiseData Noise;
    private PerlinSampler Sampler;

    public override float Evaluate(float xCoord, float yCoord)
    {
        float val = Sampler.Sample(xCoord, yCoord);
        return 1.0f - Mathf.Abs((val-0.5f)*2.0f);
    }

    protected override void OnValidate()
    {
        if (Noise != null)
        {
            Sampler = new PerlinSampler(Noise);
            Noise.OnValuesUpdated.RemoveListener(NotifyOfUpdatedValues);
            Noise.OnValuesUpdated.AddListener(NotifyOfUpdatedValues);
        }
        base.OnValidate();
    }
}
