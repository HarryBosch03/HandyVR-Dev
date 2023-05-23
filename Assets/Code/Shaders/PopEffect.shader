Shader "Unlit/PopEffect"
{
    Properties
    {
        _Color ("Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _Sharpness("Sharpness", float) = 1.0
        _Threshold("Threshold", float) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct Attributes
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct Varyings
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : VAR_COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            float4 _Color;
            float _Sharpness, _Threshold;
            
            float4 frag (Varyings i) : SV_Target
            {
                float l = length(i.uv * 2.0 - 1.0);
                float v = saturate(1.0 - l);

                float ir = 1.0 - i.color.a;
                v *= saturate(l - ir);
                
                float4 color = _Color;
                color.rgb *= i.color.rgb;

                color.a *= saturate(v * _Sharpness - _Threshold);
                clip(color.a);
                
                return color;
            }
            ENDCG
        }
    }
}
