Shader "Unlit/DoubleSidedColour"
{
	Properties
	{
		_EnvMap("Environment Map", CUBE) = "red"{}
		_MainColor("Main Colour", Color) = (1,1,1,1)
		_RimColor("Rim Colour", Color) = (1,1,1,1)
		_RimPower("Rim Power", Range(1.0, 6.0)) = 3.0
		_ReflectPow("Ref Power", Range(1.0, 6.0)) = 3.0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }

		Pass
		{
			Cull Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				float3 norm : TEXCOORD0;
				float3 posWorld : TEXCOORD2;
			};

			float4 _MainColor;
			float4 _RimColor;
			samplerCUBE _EnvMap;
			float _RimPower;
			float _ReflectPow;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				o.posWorld = mul(_Object2World, v.vertex).xyz;
				o.norm = normalize(mul((float3x3)transpose(_Object2World), v.normal));
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = _MainColor;

				// apply rim color on top
				float3 fromCam = normalize(_WorldSpaceCameraPos - i.posWorld);
				float rim = 1.0 - abs(dot(fromCam, i.norm));
				float3 cubeVector = reflect(fromCam, i.norm);

				col.rgb += (texCUBE(_EnvMap, cubeVector).rgb)*pow(rim, _ReflectPow) + _RimColor*pow(rim, _RimPower);

				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
