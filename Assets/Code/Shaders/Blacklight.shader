Shader "Unlit/Blacklight"
{
    Properties
    {
        _Value ("Brightness", float) = 2.0
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Transparent" "Queue"="Transparent"
        }
        Blend One One

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #define _ADDITIONAL_LIGHTS
            #define _ADDITIONAL_LIGHTS_SHADOWS
            #define _LIGHT_LAYERS
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            #include "BlacklightLighting.hlsl"
            
            struct Attributes
            {
                float4 vertex : POSITION;
                float4 normal : NORMAL;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 vertex : SV_POSITION;
                float3 posWS : VAR_POSITION_WS;
                float3 normal : NORMAL;

                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings vert(Attributes v)
            {
                Varyings o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                
                o.posWS = TransformObjectToWorld(v.vertex.xyz);
                o.vertex = TransformWorldToHClip(o.posWS);
                o.normal = TransformObjectToWorldDir(v.normal.xyz);
                return o;
            }

            float _Value;

            float4 frag(Varyings i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
                
                InputData inputData = (InputData)0;
                inputData.positionWS = i.posWS;
                inputData.positionCS = i.vertex;
                inputData.normalWS = normalize(i.normal);
                
                SurfaceData surfaceData = (SurfaceData)0;
                surfaceData.albedo = 1.0;

                float attenuation = BlacklightLighting(inputData, surfaceData);
                return float4(0.0, _Value * attenuation, 0.0, 0.0);
            }
            ENDHLSL
        }
    }
}