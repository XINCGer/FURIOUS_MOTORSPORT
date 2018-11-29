Shader "LightingEffects/Car/Specular_Cubemap_Color" 
{
	Properties 
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
		_Shininess ("Shininess", Range (0.01, 1)) = 0.078125
		_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
		_SpecTex ("_SpecTex", 2D) = "white" {}
		_Cube ("Cubemap", CUBE) = "" {}
		_CubePower("CubePower",Float) = 3
		_RimColor ("Fresnel Color", Color) = (0.26,0.19,0.16,0.0)
        _RimPower ("Fresnel Power", Range(0,10.0)) = 3.0
	}

	SubShader 
	{
			Tags { "RenderType"="Opaque" }
			LOD 200
			
		CGPROGRAM

		#pragma surface surf BlinnPhong

		#pragma target 3.0


		sampler2D _MainTex;
		sampler2D _SpecTex;
		half _Shininess;
		samplerCUBE _Cube;
		half _CubePower;
		half4 _Color;
		half _CubeTile;
		float4 _RimColor;
        float _RimPower;


		struct Input 
		{
			float2 uv_MainTex;
			float2 uv_SpecTex;
			float3 worldRefl;
			float3 viewDir;
		};


		void surf (Input IN, inout SurfaceOutput o) 
		{

			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
			fixed4 s_tex = tex2D(_SpecTex, IN.uv_SpecTex);
			o.Albedo = tex.rgb *_Color.rgb*1.4;
			o.Gloss = s_tex.rgb*tex.a*texCUBE (_Cube, IN.worldRefl).rgb*_CubePower;
			o.Specular = s_tex.rgb*_Shininess;

			// Rim lighting body
			half rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));
            o.Emission = UNITY_LIGHTMODEL_AMBIENT.rgb*_RimColor.rgb * pow (rim, _RimPower)*tex.rgb*1.4;
		}

		ENDCG

	}

	Fallback "MobileSpecular"

}
