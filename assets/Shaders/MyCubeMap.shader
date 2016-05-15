Shader "MyCubeMap" 
{
 	Properties 
 	{
		_CubeMap("MyCubeMap", CUBE) = "red" {}
		_LineDistance("Sight Distance", Float) = 0
		_LineThickness("Line Falloff", Float) = 2
		_OriginPoint("Origin of Sight", Vector) = (0,0,0)

	}	
    
    SubShader 
	{
  		Pass 
  		{ 
			CGPROGRAM
			
	        #pragma vertex CubeMapVS 
	        #pragma fragment CubeMapPS
	        #pragma target 3.0
	        
			#include "UnityCG.cginc"
			
			// environment map
			samplerCUBE _CubeMap;
			float4 _OriginPoint;
			float _LineDistance;
			float _LineThickness;
			
			// vertex shader input
			struct VSInput
			{
				float4 pos:		POSITION0;
				float3 norm:	NORMAL;
				float2 tex:		TEXCOORD0;
			};
			
			// vertex shader output
			struct VSOutput
			{
				float4	pos:	SV_POSITION;
				float3	norm:	TEXCOORD0;
				float3	toVert:	TEXCOORD1;
				float3	posWorld: TEXCOORD2;
			};
			
			// environment mapping vertex shader
			VSOutput CubeMapVS(VSInput a_Input)
			{
				VSOutput Output;

				// calculate homogenous vertex position
				Output.pos = mul(UNITY_MATRIX_MVP, a_Input.pos);
			
				// calculate world vertex position
				Output.posWorld = mul(_Object2World, a_Input.pos).xyz;
			
				// calculate vector from camera to vertex in world space
				Output.toVert = Output.posWorld - _WorldSpaceCameraPos;
			
				// calculate vertex normal in world space
				Output.norm = mul(transpose(_World2Object), float4(a_Input.norm, 0.0)).xyz;
							
				return Output;
			}
			
			// environment mapping pixel shader
			float4 CubeMapPS (VSOutput a_Input) : COLOR
			{				
				// calculate reflection vector to use as input to cubemap
				float3 envVector = reflect(a_Input.toVert, normalize(a_Input.norm));

				// return value from environment map
				float4 blend = texCUBE(_CubeMap, envVector);

				return blend;		
			}
		
		ENDCG
		}
	} 
}
