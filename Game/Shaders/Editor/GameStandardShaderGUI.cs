using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

class GameStandardShaderGUI : BaseShaderGUI
{
    private enum RenderingMode
    {
        Opaque,
        Cutout,
        SoftEdge,
        Transparent,
        Fade,
    }

    private static readonly string[] BlendNames = Enum.GetNames(typeof(RenderingMode));
    private MaterialProperty renderingMode;
    private MaterialProperty cutoff;
    private MaterialProperty mainTex;
    private MaterialProperty mainColor;
    private MaterialProperty emissionColor;

    private MaterialProperty specularPower;
    private MaterialProperty specularIntensity;
    private MaterialProperty specularColor;

    private MaterialProperty reflectionOpacity;
    private MaterialProperty reflectionIntensity;
    private MaterialProperty reflectionFresnel;
    private MaterialProperty reflectionMetallic;

    private MaterialProperty rimColor;
    private MaterialProperty rimIntensity;
    private MaterialProperty rimFresnel;

    private MaterialProperty rimLightColor;
    private MaterialProperty rimLightIntensity;
    private MaterialProperty rimLightFresnel;

    private bool specularEnabled;
    private bool reflectionEnabled;

    override protected void FindProperties(MaterialProperty[] props)
    {
        this.renderingMode = ShaderGUI.FindProperty("_RenderingMode", props);
        this.cutoff = ShaderGUI.FindProperty("_Cutoff", props);
        this.mainTex = ShaderGUI.FindProperty("_MainTex", props);
        this.mainColor = ShaderGUI.FindProperty("_MainColor", props);
        this.emissionColor = ShaderGUI.FindProperty("_EmissionColor", props);

        this.specularPower = ShaderGUI.FindProperty("_SpecularPower", props);
        this.specularIntensity = ShaderGUI.FindProperty("_SpecularIntensity", props);
        this.specularColor = ShaderGUI.FindProperty("_SpecularColor", props);

        this.reflectionOpacity = ShaderGUI.FindProperty("_ReflectionOpacity", props);
        this.reflectionIntensity = ShaderGUI.FindProperty("_ReflectionIntensity", props);
        this.reflectionFresnel = ShaderGUI.FindProperty("_ReflectionFresnel", props);
        this.reflectionMetallic = ShaderGUI.FindProperty("_ReflectionMetallic", props);

        this.rimColor = ShaderGUI.FindProperty("_RimColor", props);
        this.rimIntensity = ShaderGUI.FindProperty("_RimIntensity", props);
        this.rimFresnel = ShaderGUI.FindProperty("_RimFresnel", props);

        this.rimLightColor = ShaderGUI.FindProperty("_RimLightColor", props);
        this.rimLightIntensity = ShaderGUI.FindProperty("_RimLightIntensity", props);
        this.rimLightFresnel = ShaderGUI.FindProperty("_RimLightFresnel", props);
    }

    override protected void OnShaderGUI(MaterialEditor materialEditor, Material[] materials)
    {
        this.BlendModeGUI(materialEditor, materials);
        this.ColorGUI(materialEditor, materials);
        this.ReflectionGUI(materialEditor, materials);
        this.AlphaMetallicGUI(materialEditor, materials);
        this.RimGUI(materialEditor, materials);
    }

    private void BlendModeGUI(MaterialEditor materialEditor, Material[] materials)
    {
        var renderingMode = (RenderingMode)this.renderingMode.floatValue;
        EditorGUI.BeginChangeCheck();
        renderingMode = (RenderingMode)EditorGUILayout.Popup(
            "Rendering Mode", (int)renderingMode, BlendNames);
        if (EditorGUI.EndChangeCheck())
        {
           this.renderingMode.floatValue = (float)renderingMode;
            foreach (var mat in materials)
            {
                this.UpdateRenderingMode(renderingMode, mat);
            }
        }

        if (renderingMode != RenderingMode.Opaque &&
            renderingMode != RenderingMode.Transparent &&
            renderingMode != RenderingMode.Fade)
        {
            materialEditor.RangeProperty(this.cutoff, "Cutoff");
        }
    }

    private void ColorGUI(MaterialEditor materialEditor, Material[] materials)
    {
        // main texture, main color
        {
            materialEditor.TextureProperty(this.mainTex, "Main Texture");
            if (this.CheckOption(materials, "Enable Main Color", "ENABLE_MAIN_COLOR"))
            {
                EditorGUI.indentLevel = 1;
                materialEditor.ColorProperty(this.mainColor, "Main Color");
                EditorGUI.indentLevel = 0;
            }
        }

        // emission
        {
            if (this.CheckOption(materials, "Enable Emission", "ENABLE_EMISSION"))
            {
                EditorGUI.indentLevel = 1;
                materialEditor.ColorProperty(this.emissionColor, "Emission Color");
                if (this.mainTex.textureValue != null)
                {
                    this.CheckOption(
                        materials,
                        "Alpha Control",
                        "ENABLE_EMISSION_ALPHA_CONTROL");
                }

                EditorGUI.indentLevel = 0;
            }
        }

        // specular
        {
            var specularOptions = new string[] { "No Specular", "Specular", "Specular Dir" };
            var specularKeys = new string[] { "_", "ENABLE_SEPCULAR", "ENABLE_SEPCULAR_DIR" };
            if (this.ListOptions(materials, specularOptions, specularKeys) > 0)
            {
                this.specularEnabled = true;
                EditorGUI.indentLevel = 1;

                materialEditor.FloatProperty(this.specularPower, "Specular Power");
                materialEditor.FloatProperty(this.specularIntensity, "Specular Intensity");
                materialEditor.ColorProperty(this.specularColor, "Specular Color");

                EditorGUI.indentLevel = 0;
            }
            else
            {
                this.specularEnabled = false;
            }
        }
    }

    private void ReflectionGUI(MaterialEditor materialEditor, Material[] materials)
    {
        this.reflectionEnabled = this.CheckOption(
            materials,
            "Enable Reflection",
            "ENABLE_REFLECTION");

        if (!this.reflectionEnabled)
        {
            return;
        }

        EditorGUI.indentLevel = 1;

        materialEditor.RangeProperty(this.reflectionOpacity, "Opacity");
        materialEditor.RangeProperty(this.reflectionIntensity, "Intensity");
        materialEditor.RangeProperty(this.reflectionFresnel, "Fresnel");
        materialEditor.RangeProperty(this.reflectionMetallic, "Metallic");

        EditorGUI.indentLevel = 0;
    }

    private void AlphaMetallicGUI(MaterialEditor materialEditor, Material[] materials)
    {
        const string DEFINE = "ALPHA_IS_METALLIC";

        // 只有在开启了反射或者高光时才有通明通道是金属的功能
        if (!this.AlphaMetallicPreCheck())
        {
            foreach (var mat in materials)
            {
                mat.DisableKeyword(DEFINE);
            }

            return;
        }

        this.CheckOption(materials, "Alpha Is Metallic", DEFINE);
    }

    private void RimGUI(MaterialEditor materialEditor, Material[] materials)
    {
        if (this.CheckOption(materials, "Enable Rim", "ENABLE_RIM"))
        {
            EditorGUI.indentLevel = 1;
            materialEditor.ColorProperty(this.rimColor, "Color");
            materialEditor.RangeProperty(this.rimIntensity, "Intensity");
            materialEditor.RangeProperty(this.rimFresnel, "Fresnel");
            EditorGUI.indentLevel = 0;
        }

        if (this.CheckOption(materials, "Enable Rim Light", "ENABLE_RIM_LIGHT"))
        {
            EditorGUI.indentLevel = 1;
            materialEditor.ColorProperty(this.rimLightColor, "Color");
            materialEditor.RangeProperty(this.rimLightIntensity, "Intensity");
            materialEditor.RangeProperty(this.rimLightFresnel, "Fresnel");
            EditorGUI.indentLevel = 0;
        }
    }

    private bool AlphaMetallicPreCheck()
    {
        if (this.specularEnabled)
        {
            return true;
        }

        if (this.reflectionEnabled)
        {
            return true;
        }

        return false;
    }

    private void UpdateRenderingMode(
       RenderingMode renderingMode, Material material)
    {
        switch (renderingMode)
        {
            case RenderingMode.Opaque:
                material.SetInt("_SrcBlend", (int)BlendMode.One);
                material.SetInt("_DstBlend", (int)BlendMode.Zero);
                material.SetInt("_ZWrite", 1);
                material.DisableKeyword("_ALPHA_TEST");
                material.DisableKeyword("_ALPHA_BLEND");
                material.DisableKeyword("_ALPHA_PREMULTIPLY");
                material.SetOverrideTag("RenderType", "Opaque");
                material.renderQueue = -1;
                break;

            case RenderingMode.Cutout:
                material.SetInt("_SrcBlend", (int)BlendMode.One);
                material.SetInt("_DstBlend", (int)BlendMode.Zero);
                material.SetInt("_ZWrite", 1);
                // 开启alpha测试时，alpha小于culloff的值将丢掉（用clip方法），这种方法会产生边缘不平滑
                material.EnableKeyword("_ALPHA_TEST"); 
                material.DisableKeyword("_ALPHA_BLEND");
                material.DisableKeyword("_ALPHA_PREMULTIPLY");
                material.SetOverrideTag("RenderType", "TransparentCutout");
                material.renderQueue = -1;
                break;

            case RenderingMode.SoftEdge:
                material.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 1);
                material.EnableKeyword("_ALPHA_TEST");
                // 使用alphatest后再使用alphablend将使边缘平滑
                material.EnableKeyword("_ALPHA_BLEND");
                material.DisableKeyword("_ALPHA_PREMULTIPLY");
                material.SetOverrideTag("RenderType", "TransparentCutout");
                material.renderQueue = 2500;
                break;

            case RenderingMode.Transparent:
                material.SetInt("_SrcBlend", (int)BlendMode.One);
                material.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 1);
                material.DisableKeyword("_ALPHA_TEST");
                material.DisableKeyword("_ALPHA_BLEND");
                material.EnableKeyword("_ALPHA_PREMULTIPLY");
                material.SetOverrideTag("RenderType", "Transparent");
                material.renderQueue = 3000;
                break;

            case RenderingMode.Fade:
                material.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHA_TEST");
                material.EnableKeyword("_ALPHA_BLEND");
                material.DisableKeyword("_ALPHA_PREMULTIPLY");
                material.SetOverrideTag("RenderType", "Transparent");
                material.renderQueue = 3000;
                break;
        }
    }
}
