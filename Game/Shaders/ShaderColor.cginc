//------------------------------------------------------------------------------
// This file is part of MistLand project in Nirvana.
// Copyright © 2016-2016 Nirvana Technology Co., Ltd.
// All Right Reserved.
//------------------------------------------------------------------------------

#ifndef SHADERCOLOR_INCLUDED
#define SHADERCOLOR_INCLUDED

// The color for the shader.
struct ShaderColor
{
    half3 albedo;
    half3 emission;
    half3 ambience;
    half3 diffuse;
    half3 specular;
    half alpha;
    half emissionControl;
    half specularControl;
};

#define SHADER_COLOR_INITIALIZE(c) \
    c.albedo = half3(0, 0, 0); \
    c.emission = half3(0, 0, 0); \
    c.ambience = half3(0, 0, 0); \
    c.diffuse = half3(0, 0, 0); \
    c.specular = half3(0, 0, 0); \
    c.alpha = 1; \
    c.emissionControl = 1; \
    c.specularControl = 1;
    
// Color contant buffer
CBUFFER_START(ShaderColor)
    fixed _Cutoff;
CBUFFER_END

inline void applyAlpha(inout ShaderColor col)
{
// 没有 Premultiplied Alpha 的纹理无法进行 Texture Filtering（除非使用最近邻插值）。
#ifdef _ALPHA_PREMULTIPLY
    col.albedo *= col.alpha;
#endif

// 没有使用透明度测试和透明度融合且Premultiplied Alpha 则直接设alpha为1
#if !defined(_ALPHA_TEST) && !defined(_ALPHA_BLEND) && !defined(_ALPHA_PREMULTIPLY)
    UNITY_OPAQUE_ALPHA(col.alpha);
#endif

// 透明度测试
#if defined(_ALPHA_TEST)
    clip(col.alpha - _Cutoff); // alpha小于_Cutoff则丢弃该pixel
#endif
}

inline fixed4 getFinalColor(ShaderColor col, fixed atten)
{
    half3 emission = 1 + col.emission * col.emissionControl;
    half3 specular = col.specular * col.specularControl;
    half3 final = col.albedo * emission * (col.ambience + atten * (col.diffuse + specular));
    return fixed4(final.r, final.g, final.b, col.alpha);
}

#endif