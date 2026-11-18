Shader "Custom/OutlineEdge"
{
    Properties
    {
        _OutlineColor ("Outline Color", Color) = (0.6, 0.6, 0.2, 1)
        _OutlineThickness ("Outline Thickness (px)", Float) = 2
    }

    SubShader
    {
        Tags { "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            Name "OutlineEdge"
            ZTest Always
            ZWrite Off
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            float4 _OutlineColor;
            float _OutlineThickness;

            half4 frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.texcoord;
                float2 texel = _BlitTexture_TexelSize.xy * _OutlineThickness;

                float center = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv).r;
                float n = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv + float2(0, texel.y)).r;
                float s = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv - float2(0, texel.y)).r;
                float e = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv + float2(texel.x, 0)).r;
                float w = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv - float2(texel.x, 0)).r;

                float edge = saturate(abs(center - n) + abs(center - s) + abs(center - e) + abs(center - w));
                // Outline tylko na granicy sylwetki, nie w jej wnetrzu.
                edge *= (1 - center);

                return half4(_OutlineColor.rgb, edge * _OutlineColor.a);
            }
            ENDHLSL
        }
    }
}
