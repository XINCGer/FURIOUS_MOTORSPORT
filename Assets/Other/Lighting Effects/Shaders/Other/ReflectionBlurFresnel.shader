Shader "Elementals/Reflection Blur Fresnel"
{
	Properties
	{
		_MainTint ("Main Tint", Color) = (0.5,0.5,0.5,0.5)
		_MainTex ("Texture", 2D) = "white" {}
		_NormalMap ("Normal Map", 2D) = "bump" {}
		_SpecularMap("Specular Map", 2D) = "white" {}
		_Cube ("Cubemap", CUBE) = "" {}  
		_ReflectionMap ("Reflection Map", 2D) = "white" {}
		_Blur ("Blur", Float) = 0.5  
		
		_SpecColor ("Specular Color", Color) = (0.5,0.5,0.5,1)
		_Shininess ("Shininess", Range (0.01, 1)) = 0.078125 

		_RimColor("Rim Color", Color) = (1,0,0,1)
		_RimPower("Rim Power", Float) = 100

		_FresIn("Fresnel Mid", Range(0.001, 1.0)) = 0.25
		_FresOut("Fresnel Zovniwn", Range(0.001, 1.0)) = 0.25
	} 
	
	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		CGPROGRAM 
		#pragma surface surf  Sram 
		#pragma glsl
		#pragma target 3.0 
		
		float _Blur;
		float _BlurType;
		float _BlurRange;
		fixed _ReflPower; 
		
		float _Shininess; 
		float _SpecPower;
		
		float4 _FresColor;
		fixed _FresPowerIn;
		fixed _FresPowerOut;
		fixed _FresIn;
		fixed _FresOut;
				
		fixed4 _MainTint;
		float4 _RimColor; 
		float _RimPower; 
		
		struct Input
		{
			float2 uv_MainTex;
			float2 uv_NormalMap; 
			float2 uv_SpecularMap; 
			float2 uv_ReflectionMap;
			float3 worldRefl;
			float3 viewDir; 
			INTERNAL_DATA
		}; 
		
		sampler2D _MainTex;
		sampler2D _NormalMap;
		sampler2D _SpecularMap;
		sampler2D _ReflectionMap;
		samplerCUBE _Cube;  
		float4 refl;
		
		inline fixed4 LightingSram(SurfaceOutput s, fixed3 lightDir, fixed3 viewDir, fixed atten)
		{
			fixed3 halfVector = normalize(lightDir + viewDir);
		
			fixed NdotL = max(0, dot(s.Normal, lightDir));
			
			fixed EdotH = max(0, dot(viewDir, halfVector));
			fixed NdotH = max(0, dot(s.Normal, halfVector));
			fixed NdotE = max(0, dot(s.Normal, viewDir));
			
			//Rim light  
			fixed rimLight = 0;
			rimLight = pow(rimLight, _RimPower) * NdotH;
			 
			float spec = pow (NdotH, s.Specular*128.0) * s.Gloss;
			
			fixed4 finalColor;
			finalColor.rgb = (s.Albedo * _LightColor0.rgb * NdotL +  _LightColor0.rgb * spec * _SpecColor + rimLight) * (NdotE * atten * 2);
			return finalColor;
		} 
		
		float4 ReflectionBlurCube(samplerCUBE CubeTex, float3 refVector, float _Blur)
		{
			_BlurRange = _Blur/10;
				refl += texCUBE (CubeTex, refVector - float3(4*_BlurRange,0,0))*0.05;
				refl += texCUBE (CubeTex, refVector - float3(3*_BlurRange,0,0))*0.09;
				refl += texCUBE (CubeTex, refVector -float3(2*_BlurRange,0,0))*0.12;
				refl += texCUBE (CubeTex, refVector -float3(1*_BlurRange,0,0))*0.15;
				refl += texCUBE (CubeTex, refVector ) * 0.16;
				refl += texCUBE (CubeTex, refVector + float3(1*_BlurRange,0,0))*0.15;
				refl += texCUBE (CubeTex, refVector + float3(2*_BlurRange,0,0))*0.12;
				refl += texCUBE (CubeTex, refVector + float3(3*_BlurRange,0,0))*0.09;
				refl += texCUBE (CubeTex, refVector + float3(4*_BlurRange,0,0))*0.05;
	
//				refl += texCUBE (CubeTex, refVector -float3(0,4*_BlurRange,0))*0.05;
//				refl += texCUBE (CubeTex, refVector -float3(0,3*_BlurRange,0))*0.09;
//				refl += texCUBE (CubeTex, refVector -float3(0,2*_BlurRange,0))*0.12;
//				refl += texCUBE (CubeTex, refVector -float3(0,1*_BlurRange,0))*0.15;
//				refl += texCUBE (CubeTex, refVector ) * 0.16;
//				refl += texCUBE (CubeTex, refVector +float3(0,1*_BlurRange,0))*0.15;
//				refl += texCUBE (CubeTex, refVector +float3(0,2*_BlurRange,0))*0.12;
//				refl += texCUBE (CubeTex, refVector +float3(0,3*_BlurRange,0))*0.09;
//				refl += texCUBE (CubeTex, refVector +float3(0,4*_BlurRange,0))*0.05;
//	
//				refl += texCUBE (CubeTex, refVector - float3(0,0,4*_BlurRange))*0.05;
//				refl += texCUBE (CubeTex, refVector - float3(0,0,3*_BlurRange))*0.09;
//				refl += texCUBE (CubeTex, refVector - float3(0,0,2*_BlurRange))*0.12;
//				refl += texCUBE (CubeTex, refVector - float3(0,0,1*_BlurRange))*0.15;
//				refl += texCUBE (CubeTex, refVector ) * 0.16;
//				refl += texCUBE (CubeTex, refVector + float3(0,0,1*_BlurRange))*0.15;
//				refl += texCUBE (CubeTex, refVector + float3(0,0,2*_BlurRange))*0.12;
//				refl += texCUBE (CubeTex, refVector + float3(0,0,3*_BlurRange))*0.09;
//				refl += texCUBE (CubeTex, refVector + float3(0,0,4*_BlurRange))*0.05;
	
//				refl += texCUBElod (CubeTex, float4(refVector - float3(4*_BlurRange,0,0), _Blur))*0.05;
//				refl += texCUBElod (CubeTex, float4(refVector - float3(3*_BlurRange,0,0), _Blur))*0.09;
//				refl += texCUBElod (CubeTex, float4(refVector - float3(2*_BlurRange,0,0), _Blur))*0.12;
//				refl += texCUBElod (CubeTex, float4(refVector - float3(1*_BlurRange,0,0), _Blur))*0.15;
//				refl += texCUBElod (CubeTex, float4(refVector , _Blur)) * 0.16;
//				refl += texCUBElod (CubeTex, float4(refVector + float3(1*_BlurRange,0,0), _Blur))*0.15;
//				refl += texCUBElod (CubeTex, float4(refVector + float3(2*_BlurRange,0,0), _Blur))*0.12;
//				refl += texCUBElod (CubeTex, float4(refVector + float3(3*_BlurRange,0,0), _Blur))*0.09;
//				refl += texCUBElod (CubeTex, float4(refVector + float3(4*_BlurRange,0,0), _Blur))*0.05;
//	
//				refl += texCUBElod (CubeTex, float4(refVector - float3(0,4*_BlurRange,0), _Blur))*0.05;
//				refl += texCUBElod (CubeTex, float4(refVector - float3(0,3*_BlurRange,0), _Blur))*0.09;
//				refl += texCUBElod (CubeTex, float4(refVector - float3(0,2*_BlurRange,0), _Blur))*0.12;
//				refl += texCUBElod (CubeTex, float4(refVector - float3(0,1*_BlurRange,0), _Blur))*0.15;
//				refl += texCUBElod (CubeTex, float4(refVector , _Blur)) * 0.16;
//				refl += texCUBElod (CubeTex, float4(refVector + float3(0,1*_BlurRange,0), _Blur))*0.15;
//				refl += texCUBElod (CubeTex, float4(refVector + float3(0,2*_BlurRange,0), _Blur))*0.12;
//				refl += texCUBElod (CubeTex, float4(refVector + float3(0,3*_BlurRange,0), _Blur))*0.09;
//				refl += texCUBElod (CubeTex, float4(refVector + float3(0,4*_BlurRange,0), _Blur))*0.05;
//	
//				refl += texCUBElod (CubeTex, float4(refVector - float3(0,0,4*_BlurRange), _Blur))*0.05;
//				refl += texCUBElod (CubeTex, float4(refVector - float3(0,0,3*_BlurRange), _Blur))*0.09;
//				refl += texCUBElod (CubeTex, float4(refVector - float3(0,0,2*_BlurRange), _Blur))*0.12;
//				refl += texCUBElod (CubeTex, float4(refVector - float3(0,0,1*_BlurRange), _Blur))*0.15;
//				refl += texCUBElod (CubeTex, float4(refVector , _Blur)) * 0.16;
//				refl += texCUBElod (CubeTex, float4(refVector + float3(0,0,1*_BlurRange), _Blur))*0.15;
//				refl += texCUBElod (CubeTex, float4(refVector + float3(0,0,2*_BlurRange), _Blur))*0.12;
//				refl += texCUBElod (CubeTex, float4(refVector + float3(0,0,3*_BlurRange), _Blur))*0.09;
//				refl += texCUBElod (CubeTex, float4(refVector + float3(0,0,4*_BlurRange), _Blur))*0.05;
			 
				//return refl/3 ;//* _ReflPower;
				//refl = texCUBE (CubeTex, refVector);
				return refl ;//* _ReflPower;
		}
		
		void surf (Input IN, inout SurfaceOutput o)
		{ 
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb * _MainTint * c.a + c.rgb *(1- c.a);
			half4 g = tex2D (_SpecularMap, IN.uv_SpecularMap);
			o.Gloss = g.r;
			o.Specular = _Shininess; 
			o.Normal = UnpackNormal (tex2D (_NormalMap, IN.uv_NormalMap)); 
			
			_ReflPower = tex2D (_ReflectionMap, IN.uv_ReflectionMap).r;
									
 			half rim = 1 - saturate(dot(normalize(IN.viewDir), o.Normal));
 			half fr1 = 1 - saturate(dot(normalize(IN.viewDir), o.Normal));
 			half fr2 = saturate(dot(normalize(IN.viewDir), o.Normal));

			float4 bluredReflection = ReflectionBlurCube(_Cube,WorldReflectionVector (IN, o.Normal),_Blur);
			o.Emission = bluredReflection * _ReflPower *_FresOut * pow(fr1, 1) + 
						 bluredReflection * _ReflPower *_FresIn * pow(fr2, 1) + 
						 _RimColor.rgb * pow(rim, _RimPower)*_RimColor.a;
		}
		ENDCG
	} 
	Fallback "Diffuse"
}