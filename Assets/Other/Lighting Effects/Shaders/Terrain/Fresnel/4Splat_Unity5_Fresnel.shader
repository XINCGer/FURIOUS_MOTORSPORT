// Orginally writed by ALIyerEdon Summer-Fall 2016
Shader "LightingEffects/Terrain/4Splat_Unity5_Fresnel" 
{
	Properties
	{
		// Shader properties that you can see and change it on inspector
		_Control ("Control (RGBA)", 2D) = "red" {}
		_Splat0 ("Ch (R)", 2D) = "white" {}
		_Splat1 ("Ch (G)", 2D) = "white" {}
		_Splat2 ("Ch (B)", 2D) = "white" {}
		_Splat3 ("Ch (A)", 2D) = "white" {}
		_RimColor ("Fresnel Color", Color) = (0.26,0.19,0.16,0.0)
        _RimPower ("Fresnel Power", Range(0.5,8.0)) = 3.0
		_UV1("Splat1 & Splat3 Tile",Float) =   43
		_UV2("Splat2 & Splat4 Tile",Float) =   43
	}
		
	SubShader // Main shader body
	{
		Tags 
		{
			"SplatCount" = "4"  // Need 4 splat for terrain
			"Queue" = "Geometry-100" 
			"RenderType" = "Opaque" // This is Opaque and is not Transparent shader
		}
		

    LOD 150
	CGPROGRAM

	// Use Lambert lighting
	#pragma surface surf Lambert vertex:vert

	// Can run on GLES2+ - Works on all platforms supported by Unity
	#pragma target 2.0

	struct Input 
	{
		float2 uv_Control : TEXCOORD0;
		float2 customUV1;float2 customUV2;
		float3 viewDir; 
	};

	// This is reWriteded properties on top for use in shader codes
	sampler2D _Control;
	sampler2D _Splat0,_Splat1,_Splat2,_Splat3;
	half _UV1;
	half _UV2;
	float4 _RimColor;
    float _RimPower;

	// Vertex shader
	void vert (inout appdata_full v, out Input o) 
	{
		UNITY_INITIALIZE_OUTPUT(Input,o);

		// Custom UV is unreal engine way for solving mali400 pixelled tiling problem
        // And use this for more performance compare to pixel shader   
		o.customUV1 = v.texcoord.xy *_UV1;o.customUV2 = v.texcoord.xy *_UV2;
	}

	// Pixel shader
	void surf (Input IN, inout SurfaceOutput o) 
	{
		float4 splat_control = tex2D (_Control, IN.uv_Control);
		float3 col;
		col  = splat_control.r * tex2D (_Splat0, IN.customUV1).rgb;
		col += splat_control.g * tex2D (_Splat1, IN.customUV2).rgb;
		col += splat_control.b * tex2D (_Splat2, IN.customUV1).rgb;
		col += splat_control.a * tex2D (_Splat3, IN.customUV2).rgb;
		o.Albedo = col;

		// Rim lighting body
		half rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));
        o.Emission = _RimColor.rgb * pow (rim, _RimPower);

	}
	ENDCG  
	}

	Fallback "Diffuse"
}
