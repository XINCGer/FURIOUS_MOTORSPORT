// Orginally writed by ALIyerEdon Summer-Fall 2016
Shader "LightingEffects/Mobile/Diffuse_Detail" 
{
	Properties
	 {
	 	 // Shader properties that you can see and change it on inspector
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_DetailTex ("Detail (RGB)", 2D) = "white" {}
		_DetailUVTile("Detail UV Tile",Float) = 1
	}

	SubShader // Main shader body
	{
			Tags { "RenderType"="Opaque" }
			LOD 150
			
		CGPROGRAM

		// BlinnPhong lighting for specular shaders + vert function for vertex shader program
		#pragma surface surf Lambert vertex:vert 

		// Can run on GLES2+ - Works on all platforms supported by Unity
		#pragma target 2.0

		// This is reWriteded properties on top for use in shader codes
		sampler2D _MainTex;
		sampler2D _DetailTex;
		float _DetailUVTile   ;


		struct Input {
			float2 uv_MainTex;
			float2 customUV1;    
		};

		// Vertex shader
		void vert (inout appdata_full v, out Input o)
		 {
		          UNITY_INITIALIZE_OUTPUT(Input,o);

		          // Custom UV is unreal engine way for solving mali400 pixelled tiling problem
	         	  // And use this for more performance compare to pixel shader 
		          o.customUV1 = v.texcoord.xy * _DetailUVTile;
		 }



		// Pixel shader
		void surf (Input IN, inout SurfaceOutput o) 
		{
			// Main texture
			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);

			// Detail texture
			fixed4 d_tex = tex2D(_DetailTex, IN.customUV1);

			// Albedo main texture *detail texture
			o.Albedo = tex.rgb *d_tex.rgb*2;
		}

		ENDCG

	}

	Fallback "Diffuse"
}
