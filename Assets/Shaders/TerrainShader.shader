Shader "Custom/TerrainShader"
{
    Properties
    {
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        const static int MAX_COLOURS = 8;

        int baseColourCount;
        float3 baseColours[MAX_COLOURS];
        float baseStartHeights[MAX_COLOURS];
        float baseBlendStrength[MAX_COLOURS];

        float minHeight;
        float maxHeight;

        struct Input
        {
            float3 worldPos;
        };

        float inverseLerp(float val, float min, float max)
        {
            float ret = (val - min) / (max - min);
            return ret < 0 ? 0 : ret > 1 ? 1 : ret;
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float heightPercent = inverseLerp(IN.worldPos.y, minHeight, maxHeight);
            for (int i = 0; i < baseColourCount; i++)
            {
                float drawStrength = heightPercent >= baseStartHeights[i] ? 1.0 : 0.0;
                o.Albedo = o.Albedo * (1-drawStrength) + baseColours[i] * drawStrength;
            }
        }
        ENDCG
    }
    FallBack "Diffuse"
}
