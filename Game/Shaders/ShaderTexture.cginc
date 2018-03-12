#ifndef SHADERTEXTURE_INCLUDED
#define SHADERTEXTURE_INCLUDED

#include "ShaderColor.cginc"

CBUFFER_START(ShaderTexture)
	sampler2D _MainTex;
	half4 _MainTex_ST;
	fixed4 _MainColor;
	half4 _EmissionColor;

	sampler2D _MaskControlTex;
CBUFFER_END

#if defined(LIGHTMAP_ON)
	#define SHADER_TEXCOORDS_TYPE half4
	#define SHADER_TEXCOORDS(sematic) half4 uv : sematic;
#else
	#define SHADER_TEXCOORDS_TYPE half2
	#define SHADER_TEXCOORDS(sematic) half2 uv : sematic;

#endif

// TRANSFER_UV
#if defined(LIGHTMAP_ON)
	#define TRANSFER_UV(o, v) \
        o.uv.xy = TRANSFORM_TEX(v.uv, _MainTex); \
		o.uv.zw = v.uv2 * unity_LightmapST.xy + unity_LightmapST.zw;
#else
	#define TRANSFER_UV(o, v) \
        o.uv = TRANSFORM_TEX(v.uv, _MainTex);
#endif

inline void applyTexture(SHADER_TEXCOORDS_TYPE uv, inout ShaderColor col)
{
	// 从纹理中提取出某UV坐标下的颜色
	fixed4 mainColor = tex2D(_MainTex, uv.xy);
	// 反射率
	col.albedo = mainColor.rgb;
	// 不是金属
#ifndef ALPHA_IS_METALLIC
	col.alpha = mainColor.a;
#endif

	// 使用主颜色，与反射率相乘
#ifdef ENABLE_MAIN_COLOR
	col.albedo *= _MainColor.rgb;
	col.alpha *= _MainColor.a;
#endif

	// 应用Alpha
	applyAlpha(col);

	// 外发光颜色
#if ENABLE_EMISSION
	col.emission = _EmissionColor;
#endif
	
	// 光照贴图
#ifdef LIGHTMAP_ON
	fixed4 lmtex = UNITY_SAMPLE_TEX2D(unity_Lightmap, uv.zw);
	col.albedo *= DecodeLightmap(lmtex);
#endif

	// 用另一个纹理上的R和G通道代表specularControl和emissionControl
#if ENABLE_MASK_CONTROL
	fixed4 mask = tex2D(_MaskControlTex, uv.xy);
	col.specularControl = mask.r;
	col.emissionControl = mask.g;
#endif

	// tag格式贴图里带了aplha通道。alpha通道的值作为高光强度值
#ifdef ALPHA_IS_METALLIC
	col.specularControl *= mainColor.a;
#endif

	// tga格式贴图里带了alhpa通道，alpha通道的值作为自发光的强度值
#ifdef ENABLE_EMISSION_ALPHA_CONTROL
	col.emissionControl *= mainColor.a;
#endif
}

#endif