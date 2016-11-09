/**
 * Portal shader.
 * Based on the Unity Pro water asset.
 */

Shader "FX/Portal" {
Properties {
	_Color("Fallback color", Color) = (0, 0, 0, 1)
	_PortalTex("Rendered Portal", 2D) = "" {}
	_Scale("Scale center, mode, scale", vector) = (
		//x/y center of scaling
		0, 0,
		//mode: zero=portal, positive=opaque color, negative=invisible
		0,
		//how much to scale
		1
	)
}


// -----------------------------------------------------------
// Fragment program cards


Subshader {
	Tags { "RenderType"="Opaque" }
	Pass {
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest

#include "UnityCG.cginc"

struct appdata {
	float4 vertex : POSITION;
	float3 normal : NORMAL;
};

struct v2f {
	float4 pos : SV_POSITION;
	float4 ref : TEXCOORD0;
};


v2f vert(appdata v)
{
	v2f o;
	o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
	o.ref = ComputeScreenPos(o.pos);
	return o;
}

uniform sampler2D _PortalTex;
uniform float4 _Color;
uniform float4 _Scale;

half4 frag(v2f i) : COLOR
{
	if (_Scale.z < 0) discard;
	else if (_Scale.z > 0) return _Color;

	float2 pos = i.ref.xy / i.ref.w;
	float4 pOffset = float4(_Scale.xy, 0, 0);
	pos.xy = (pos.xy - pOffset) / _Scale.w + pOffset;

	//return tex2D( _PortalTex, pos) * .9 + .1*_Color;
	return tex2D( _PortalTex, pos);

//	float4 pos = i.ref;
//	float4 pOffset = float4(_Scale.xy, 0, 0);
//	pos.xy = (pos.xy - pOffset) * _Scale.w + pOffset;
//
//	return tex2Dproj( _PortalTex, UNITY_PROJ_COORD(pos));

	//return tex2D( _PortalTex, i.ref.xy / i.ref.w);
	//return tex2Dproj( _PortalTex, UNITY_PROJ_COORD(i.ref));
}
ENDCG

	}
}

// -----------------------------------------------------------
//  Old cards

// single texture
Subshader {
	Tags { "RenderType"="Opaque" }
	Pass {
		SetTexture [_MainTex] {
			combine texture
		}
	}
}


}
