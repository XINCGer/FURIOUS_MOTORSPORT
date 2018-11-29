// Orginally writed by ALIyerEdon Summer-Fall 2016
Shader "LightingEffects/Legacy/Diffuse_Specular_Detail" {
	Properties
	 {
	 	// Shader properties that you can see and change it on inspector
		_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
		_SpecPower("Specular Power",Float)=     4
		_Shininess ("Shininess", Range (0.01, 1)) = 0.078125
		_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
		_SpecTex ("_SpecTex", 2D) = "white" {}
		_DetailTex ("_DetailTex", 2D) = "white" {}
		_RimColor ("Fresnel Color", Color) = (0.26,0.19,0.16,0.0)
        _RimPower ("Fresnel Power", Range(0,10.0)) = 3.0

	}
	SubShader // Main shader body
	{
		Tags { "RenderType"="Opaque" } // This is Opaque and is not Transparent shader
		LOD 200
		
		CGPROGRAM

		// BlinnPhong lighting for specular shaders + vert function for vertex shader program
		#pragma surface surf BlinnPhong    

		// Can run on GLES2+ - Works on all platforms supported by Unity
		#pragma target 2.0

		// This is reWriteded properties on top for use in shader codes
		sampler2D _MainTex;
		sampler2D _SpecTex;
		half _Shininess;
		sampler2D _DetailTex;
		half _SpecPower;
		float4 _RimColor;
        float _RimPower;

		struct Input 
		{
			float2 uv_MainTex;
			float2 uv_DetailTex;
			float3 viewDir;
		};



     // Pixel shader
	  void surf (Input IN, inout SurfaceOutput o) 
	  {
	  		// Main texture
			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);

			// Specular map texture
			fixed4 s_tex = tex2D(_SpecTex, IN.uv_MainTex);

			// Detail texture
			fixed4 d_tex = tex2D(_DetailTex, IN.uv_DetailTex);

			// Main texture * detail texture
			o.Albedo = tex.rgb *d_tex.rgb*2;

			// Specular map texture * detail texture
			o.Gloss = s_tex.rgb*tex.a*d_tex.rgb*_SpecPower;

			// Specular map texture * shininess
			o.Specular = s_tex.rgb*_Shininess;

			// Rim lighting body
			half rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));
            o.Emission = UNITY_LIGHTMODEL_AMBIENT.rgb*_RimColor.rgb * pow (rim, _RimPower)*tex.rgb*1.4;

		}
		ENDCG
	}
	FallBack "Diffuse"
}

