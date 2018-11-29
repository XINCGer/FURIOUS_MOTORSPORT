// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "LightingEffects/Lighting/Flare_Color" {
Properties {
	_Color("Color",Color) = (1,1,1,1)
	_MainTex ("Particle Texture", 2D) = "black" {}

}
SubShader {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Cull Off Lighting Off ZWrite Off Ztest Always Fog { Mode Off }
	Blend One One

	Pass {	
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma fragmentoption ARB_precision_hint_fastest

		#include "UnityCG.cginc"

		sampler2D _MainTex;
		fixed4 _Color;
		
		struct appdata_t {
			float4 vertex : POSITION;
			fixed4 color : COLOR;
			float2 texcoord : TEXCOORD0;
		};

		struct v2f {
			float4 vertex : POSITION;
			fixed4 color : COLOR;
			float2 texcoord : TEXCOORD0;
		};

		float4 _MainTex_ST;
		
		v2f vert (appdata_t v)
		{
			v2f o;
			o.vertex = UnityObjectToClipPos(v.vertex);
			o.color = v.color;
			o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
			return o;
		}

		fixed4 frag (v2f i) : COLOR
		{
			fixed4 col;
			fixed4 tex = tex2D(_MainTex, i.texcoord);
			col.rgb = i.color.rgb * tex.rgb*_Color.rgb;
			col.a = tex.a;
			return col;
		}
		ENDCG 
	}
} 	

SubShader {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Cull Off Lighting Off ZWrite Off Ztest Always Fog { Mode Off }
	Blend One One
	Color (1,1,1,1)
	Pass {
		SetTexture [_MainTex] { combine texture * primary, texture }
	}
}
}
