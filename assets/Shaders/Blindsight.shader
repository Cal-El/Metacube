Shader "Blindsight" 
{
 	Properties 
 	{
		_LineDistance("Sight Distance", Float) = 0
		_LineThickness("Line Falloff", Float) = 2
		_OriginPoint("Origin of Sight", Vector) = (0,0,0)
		_ThisColour("Object Colour", Color) = (1,1,1,1)
	}	
    
    SubShader 
	{
  		Pass 
  		{ 
			CGPROGRAM
			
	        #pragma vertex BlindsightVS 
	        #pragma fragment BlindsightPS
	        #pragma target 3.0
	        
			#include "UnityCG.cginc"
			
			// environment map
			float4 _OriginPoint;
			float _LineDistance;
			float _LineThickness;
			float4 _ThisColour;
			
			// vertex shader input
			struct VSInput
			{
				float4 pos:		POSITION0;
			};
			
			// vertex shader output
			struct VSOutput
			{
				float4	pos:	SV_POSITION;
				float3	posWorld	: TEXCOORD0;
			};
			
			// environment mapping vertex shader
			VSOutput BlindsightVS(VSInput a_Input)
			{
				VSOutput Output;

				// calculate homogenous vertex position
				Output.pos = mul(UNITY_MATRIX_MVP, a_Input.pos);
			
				// calculate world vertex position
				Output.posWorld = mul(_Object2World, a_Input.pos).xyz;

				return Output;
			}
			
			// environment mapping pixel shader
			float4 BlindsightPS (VSOutput a_Input) : COLOR
			{				
				float4 colour = _ThisColour;
				float halfThick = _LineThickness / 2;
				float distanceFromOrigin = length(a_Input.posWorld - _OriginPoint.xyz);
				colour = colour * max((1-pow(distanceFromOrigin - _LineDistance, 2)/pow(halfThick,2)), 0.0);
				return colour;
			}
		
		ENDCG
		}
	} 
}
