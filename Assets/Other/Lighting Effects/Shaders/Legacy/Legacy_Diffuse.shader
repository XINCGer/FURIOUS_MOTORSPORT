// Orginally writed by ALIyerEdon Summer-Fall 2016
Shader "LightingEffects/Legacy/Diffuse" 
{
	Properties 
	{
		// Shader properties that you can see and change it on inspector
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_RimColor ("Fresnel Color", Color) = (0.5, 0.5, 0.5, 1)
        _RimPower ("Fresnel Power", Range(0,10.0)) = 4.3
	}
	SubShader // Main shader body
	{

		Tags { "RenderType"="Opaque" }
		LOD 150
		
		CGPROGRAM

		// Lambert lighting for diffuse shaders
		#pragma surface surf Lambert

		// Can run on GLES2+ - Works on all platforms supported by Unity
		#pragma target 2.0

		// This is reWriteded properties on top for use in shader codes
		sampler2D _MainTex;
		fixed4 _Color;
		float4 _RimColor;
        float _RimPower;

		struct Input 
		{
			float2 uv_MainTex;float3 viewDir;
		};


        // Pixel shader
		void surf (Input IN, inout SurfaceOutput o) 
		{


			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;

			// Rim lighting body     
			half rim = 1.0 - saturate(dot (normalize(IN.viewDir/2), o.Normal));
            o.Emission = UNITY_LIGHTMODEL_AMBIENT.rgb*_RimColor.rgb * pow (rim, _RimPower)*c.rgb*1.4;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
