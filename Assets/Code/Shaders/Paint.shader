Shader "Hidden/Paint"
{
	Properties
	{
		_BPos ("BPos", Vector) = (0.0, 0.0, 0.0, 0.0)
		_BColor ("BColor", Color) = (0.0, 0.0, 0.0, 0.0)
		_BSize ("BSize", float) = 0.0
		_BHardness ("BHardness", float) = 0.0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		ZTest Always
		ZWrite Off
		Cull Off
		
		Pass
		{
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/core.hlsl"

			struct Attributes
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD1;
			};

			struct Varyings
			{
				float4 vertex : SV_POSITION;
				float3 worldPos : VAR_WORLDPOS;
				float2 uv : TEXCOORD1;
			};

			Varyings vert (Attributes v)
			{
				Varyings o;
				o.worldPos = TransformObjectToWorld(v.vertex);
				o.uv = v.uv;
				float4 uv = float4(0.0, 0.0, 0.0, 1.0);
				uv.xy = (v.uv * 2.0 - 1.0) * float2(1.0, _ProjectionParams.x);
				o.vertex = uv;
				return o;
			}

			float4 _BPos;
			float4 _BColor;
			float _BSize;
			float _BHardness;

			TEXTURE2D(_Behind);
			SAMPLER(sampler_Behind);
			
			float4 frag (Varyings i) : SV_Target
			{
				float distance = length(i.worldPos - _BPos);
				float falloff = clamp((_BSize - distance) / (_BSize * (1 - _BHardness)), 0.0, 1.0);

				float4 behind = SAMPLE_TEXTURE2D(_Behind, sampler_Behind, i.uv);
				float4 color = lerp(behind, _BColor, _BColor.a * falloff);
				
				return color;
			}
			ENDHLSL
		}
	}
}