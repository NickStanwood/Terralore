Shader "Custom/HeatShader"
{
    Properties
    {
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
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

        struct Input
        {
            float3 worldPos;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {

        }
        ENDCG
    }
    FallBack "Diffuse"
}