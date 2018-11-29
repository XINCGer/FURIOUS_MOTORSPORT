// Orginally writed by ALIyerEdon Summer-Fall 2016
Shader "LightingEffects/Legacy/Diffuse_Specular_Normal" 
	{
	Properties 
	{
		// Shader properties that you can see and change it on inspector
		_Color ("Main Color", Color) = (1,1,1)
		_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
		_SpecPower("Specular Power",Float)=     4
		_Shininess ("Shininess", Range (0.01, 1)) = 0.078125
		_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
		_BumpMap ("Bumpmap", 2D) = "bump" {}
		_SpecTex ("_SpecTex", 2D) = "white" {}
		_RimColor ("Fresnel Color", Color) = (0.26,0.19,0.16,0.0)
        _RimPower ("Fresnel Power", Range(0,10.0)) = 3.0
	}

	SubShader // Main shader body
	{
		Tags { "RenderType"="Opaque" }// This is Opaque and is not Transparent shader
		LOD 150
			
		CGPROGRAM

		// BlinnPhong lighting for specular shaders
		#pragma surface surf BlinnPhong

		// Can run on shader model 2+
		#pragma target 2.0

		// This is reWriteded properties on top for use in shader codes
		sampler2D _MainTex;
		sampler2D _SpecTex;
		half _Shininess;
		half _SpecPower;
		sampler2D _BumpMap;
		half3 _Color;
		float4 _RimColor;
        float _RimPower;

		struct Input 
		{
			float2 uv_MainTex;  
			float2 uv_BumpMap;
			float3 viewDir;
		};

		// Pixel shader
		void surf (Input IN, inout SurfaceOutput o) 
		{
			// Main texture
			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);

			// Specular map texture
			fixed4 s_tex = tex2D(_SpecTex, IN.uv_MainTex);

			// Normal map texture
			fixed4 n_tex = tex2D(_BumpMap, IN.uv_BumpMap);

			// Albedo main texture * detail texture * color
			o.Albedo = tex.rgb *_Color.rgb;

			// unpack normal map texture
			o.Normal = UnpackNormal (n_tex);

			// Specular map texture * normal map texture * specular power   
			o.Gloss = s_tex.rgb*tex.a*_SpecPower*n_tex*10;

			// Specular map texture * shininess
			o.Specular = s_tex.rgb*_Shininess;

			// Rim lighting body
			half rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));
            o.Emission = _RimColor.rgb * pow (rim, _RimPower)*tex.rgb;
		}
		ENDCG
	}

	Fallback "Diffuse"
}

