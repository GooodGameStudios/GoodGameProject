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

            // Liczba probek na pierscieniu dylatacji - wiecej probek = gladszy
            // (mniej "dziurawy" na ostrych rogach sylwetki) ale drozszy pass.
            #define OUTLINE_RING_SAMPLES 12

            half4 frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.texcoord;
                float2 texelSize = _BlitTexture_TexelSize.xy;

                float center = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv).r;

                // Dylatacja maski: probkujemy pierscien (dwa promienie, pelny i 60%)
                // wokol kazdego piksela w odleglosci _OutlineThickness pikseli i
                // bierzemy maksimum. W przeciwienstwie do pojedynczej probki N/S/E/W
                // (stara wersja) daje to spojne PASMO o szerokosci _OutlineThickness
                // przylegajace do sylwetki, a nie waski pierscien odsuniety od niej.
                float dilated = center;
                [unroll]
                for (int i = 0; i < OUTLINE_RING_SAMPLES; i++)
                {
                    float angle = i * (6.2831853 / OUTLINE_RING_SAMPLES);
                    float2 dir = float2(cos(angle), sin(angle));

                    float2 offsetOuter = dir * texelSize * _OutlineThickness;
                    float2 offsetInner = offsetOuter * 0.6;

                    dilated = max(dilated, SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv + offsetOuter).r);
                    dilated = max(dilated, SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv + offsetInner).r);
                }

                // Pasmo outline'u = w zasiegu dylatacji, ale poza oryginalna sylwetka.
                // smoothstep (zamiast ostrego progu/saturate) wygladza probkowanie
                // blisko granicy sylwetki, lagodzac "skakanie" krawedzi o cala
                // klatke pikselowa przy drobnych ruchach kamery/obiektu.
                float band = smoothstep(0.0, 0.5, dilated - center);

                return half4(_OutlineColor.rgb, band * _OutlineColor.a);
            }
            ENDHLSL
        }
    }
}
