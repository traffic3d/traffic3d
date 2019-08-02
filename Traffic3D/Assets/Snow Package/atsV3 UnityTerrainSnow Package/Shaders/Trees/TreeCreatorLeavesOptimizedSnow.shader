Shader "Hidden/Nature/Tree Creator Leaves Optimized Snow" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_TranslucencyColor ("Translucency Color", Color) = (0.73,0.85,0.41,1) // (187,219,106,255)
	_Cutoff ("Alpha cutoff", Range(0,1)) = 0.3
	_TranslucencyViewDependency ("View dependency", Range(0,1)) = 0.7
	_ShadowStrength("Shadow Strength", Range(0,1)) = 0.8
	_ShadowOffsetScale ("Shadow Offset Scale", Float) = 1
	
	_MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
	_ShadowTex ("Shadow (RGB)", 2D) = "white" {}
	_BumpSpecMap ("Normalmap (GA) Spec (R) Shadow Offset (B)", 2D) = "bump" {}
	_TranslucencyMap ("Trans (B) Gloss(A)", 2D) = "white" {}
	
	// These are here only to provide default values
	_Scale ("Scale", Vector) = (1,1,1,1)
	_SquashAmount ("Squash", Float) = 1
}

SubShader { 
	Tags {
		"IgnoreProjector"="True"
		"RenderType"="TreeLeaf"
	}
	LOD 200
	
CGPROGRAM
#pragma surface surf TreeLeaf alphatest:_Cutoff vertex:TreeVertLeafSnow nolightmap
#pragma exclude_renderers flash
#pragma glsl_no_auto_normalization
#pragma target 3.0

#include "TreeSnow.cginc"

sampler2D _MainTex;
sampler2D _BumpSpecMap;
sampler2D _TranslucencyMap;

// all these have to be send via script = CustomTerrainScriptAtsV3Snow.cs
sampler2D _SnowTexture;
float _snowShininess;
float _SnowAmount;
float _SnowStartHeight;


struct Input {
	float2 uv_MainTex;
	fixed4 color : COLOR; // color.a = AO
	float3 worldPos;
};

void surf (Input IN, inout LeafSurfaceOutput o) {
	fixed4 col = tex2D(_MainTex, IN.uv_MainTex);
	fixed4 trngls = tex2D (_TranslucencyMap, IN.uv_MainTex);
	o.Translucency = trngls.b;
	o.Alpha = col.a;
	
	// get snow texture
	half3 snowtex = tex2D( _SnowTexture, IN.uv_MainTex).rgb;
	
	// lerp = allows snow even on orthogonal surfaces // (1-col.g) = take the blue channel to get some kind of heightmap // worldNormal is stored in IN.color
	float snowAmount = lerp(_SnowAmount * IN.color.y, 1, _SnowAmount) * (1-col.g) * .65 + o.Normal.y * _SnowAmount *.25 * IN.color.a * trngls.b;
	
	// clamp snow to _SnowStartHeight
	// billboards do not get effected by snowStartHeight anyway...
	snowAmount = snowAmount * clamp((IN.worldPos.y - _SnowStartHeight)*.0125, 0, 1);
	
	// sharpen snow mask
	snowAmount = clamp( pow(snowAmount,6)*256, 0, 1);
	
	// mix all together
	o.Gloss = trngls.a * _Color.r * (1-snowAmount) + ((1-snowtex) * snowAmount);
	o.Albedo = (col.rgb * (1-snowAmount) + snowtex.rgb*snowAmount) * IN.color.a;
	half4 norspc = tex2D (_BumpSpecMap, IN.uv_MainTex);
	o.Specular = norspc.r * (1-snowAmount) + _snowShininess * snowAmount;
	o.Normal = UnpackNormalDXT5nm(norspc);

	// smooth normal
	o.Normal = normalize(lerp(o.Normal, float3(0,0,1), snowAmount*.50));
}
ENDCG

	// Pass to render object as a shadow caster
	Pass {
		Name "ShadowCaster"
		Tags { "LightMode" = "ShadowCaster" }
		
		Fog {Mode Off}
		ZWrite On ZTest LEqual Cull Off
		Offset 1, 1

		CGPROGRAM
		#pragma vertex vert_surf
		#pragma fragment frag_surf
		#pragma exclude_renderers noshadows flash
		#pragma glsl_no_auto_normalization
		#pragma fragmentoption ARB_precision_hint_fastest
		#pragma multi_compile_shadowcaster
		#include "HLSLSupport.cginc"
		#include "UnityCG.cginc"
		#include "Lighting.cginc"

		#define INTERNAL_DATA
		#define WorldReflectionVector(data,normal) data.worldRefl

		#include "TreeSnow.cginc"

		sampler2D _ShadowTex;

		struct Input {
			float2 uv_MainTex;
		};

		struct v2f_surf {
			V2F_SHADOW_CASTER;
			float2 hip_pack0 : TEXCOORD1;
		};
		float4 _ShadowTex_ST;
		v2f_surf vert_surf (appdata_full v) {
			v2f_surf o;
			TreeVertLeafSnow (v);
			o.hip_pack0.xy = TRANSFORM_TEX(v.texcoord, _ShadowTex);
			TRANSFER_SHADOW_CASTER(o)
			return o;
		}
		fixed _Cutoff;
		float4 frag_surf (v2f_surf IN) : COLOR {
			half alpha = tex2D(_ShadowTex, IN.hip_pack0.xy).r;
			clip (alpha - _Cutoff);
			SHADOW_CASTER_FRAGMENT(IN)
		}
		ENDCG
	}
}

Dependency "BillboardShader" = "Hidden/Nature/Tree Creator Leaves Rendertex Snow"
}
