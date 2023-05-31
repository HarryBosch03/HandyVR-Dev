Shader "Unlit/AVHand"
{
    Properties
    {
        _Color("Color", Color) = (1, 1, 1, 1)
        _Value("Brightness", float) = 0.5
        _Pow("Fresnel Power", float) = 4.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        ZWrite Off
        ZTest Always
        Blend One One

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            struct Attributes
            {
                float4 vertex : POSITION;
                float4 normal : NORMAL;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 vertex : SV_POSITION;
                float3 position : VAR_POSITION;
                float3 normal : VAR_NORMAL;

                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            float4 _Color;

            float _Value, _Pow;
            
            Varyings vert (Attributes v)
            {
                Varyings o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                
                o.vertex = TransformObjectToHClip(v.vertex);
                o.position = TransformObjectToWorld(v.vertex);
                o.normal = TransformObjectToWorldDir(v.normal);
                return o;
            }

            float4 frag (Varyings i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);
                
                float3 viewDir = normalize(_WorldSpaceCameraPos - i.position);
                float ndv = dot(viewDir, normalize(i.normal));
                float3 col = _Color * pow(1.0 - ndv, _Pow) * _Value * pow(2, _Value);
                
                return max(float4(col, 1.0), 0.0);
            }
            ENDHLSL
        }
    }
}
