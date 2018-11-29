// Orginally writed by ALIyerEdon Summer-Fall 2016
Shader "LightingEffects/Mobile/Diffuse_Specular" {
	Properties
	 {
	 	// Shader properties that you can see and change it on inspector
		_Color ("Color", Color) = (1,1,1,1)
		_SpecColor ("Specular Color", Color) = (1, 1, 1, 1)
		_SpecPower("Specular Power",Float)=     3
		_Shininess ("Shininess", Range (0.01, 1)) = 1
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_SpecTex ("_SpecTex", 2D) = "white" {}
		_SpecUVTile("Specular Texure Tile",Float) = 1
	}
	SubShader // Main shader body
	{
		Tags { "RenderType"="Opaque" } // This is Opaque and is not Transparent shader
		LOD 150
		
		CGPROGRAM

		// BlinnPhong lighting for specular shaders + vert function for vertex shader program
		#pragma surface surf BlinnPhong vertex:vert

		// Can run on GLES2+
		#pragma target 2.0 - Works on all platforms supported by Unity

		// This is reWriteded properties on top for use in shader codes
		sampler2D _MainTex;
		sampler2D _SpecTex;
		half _Shininess;
		half _SpecUVTile   ;
		half _SpecPower;
		fixed4 _Color;


		struct Input 
		{
			float2 uv_MainTex;
			float2 customUV1; 
		};



	  // Vertex shader
      void vert (inout appdata_full v, out Input o) 
      {
          UNITY_INITIALIZE_OUTPUT(Input,o);

          // Custom UV is unreal engine way for solving mali400 pixelled tiling problem
          // And use this for more performance compare to pixel shader      
          o.customUV1 = v.texcoord.xy * _SpecUVTile;
      }

      // Pixel shader
	  void surf (Input IN, inout SurfaceOutput o) 
	  {
	 	    // Albedo (main texture)
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			 
			// Gloss and Specular (specular map texture and power+shininess)
			fixed4 s_tex = tex2D(_SpecTex, IN.customUV1);
            o.Gloss = s_tex.rgb*c.a*_SpecPower;
            o.Specular = s_tex.rgb*_Shininess;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
