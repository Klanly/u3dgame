using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

class GameParticleShaderGUI : BaseShaderGUI
{
    private static readonly string[] BlendNames = Enum.GetNames(typeof(RenderingMode));

    private MaterialProperty renderingMode;
    private MaterialProperty cullMode;
    private MaterialProperty cutoff;
    private MaterialProperty zwrite;

    private MaterialProperty mainTex;
    private MaterialProperty tintColor;

    private MaterialProperty decalTex;  // 贴花纹理

    private MaterialProperty dissloveTex; // 溶解
    private MaterialProperty dissloveAmount; // 溶解量

    private MaterialProperty uvNoise;
    private MaterialProperty uvNoiseBias;
    private MaterialProperty uvNoiseIntensity;
    private MaterialProperty uvNoiseSpeed;

    private MaterialProperty glowTex;
    private MaterialProperty glowSpeed;
    private MaterialProperty glowColor;

    private MaterialProperty rimColor;
    private MaterialProperty rimIntensity;
    private MaterialProperty rimFresnel;

    private MaterialProperty rimLightColor;
    private MaterialProperty rimLightIntensity;
    private MaterialProperty rimLightFresnel;

    private Texture2D mainViewTex;
    private Texture2D decalViewTex;

    private enum RenderingMode
    {
        Opaque = 3,
        Cutout = 2,
        AlphaBlend = 0,
        Additive = 1,
    }

    protected override void FindProperties(MaterialProperty[] props)
    {
        this.renderingMode = ShaderGUI.FindProperty("_RenderingMode", props);
        this.cullMode = ShaderGUI.FindProperty("_CullMode", props);
        this.cutoff = ShaderGUI.FindProperty("_Cutoff", props);
        this.zwrite = ShaderGUI.FindProperty("_ZWrite", props);
        this.mainTex = ShaderGUI.FindProperty("_MainTex", props);
        this.tintColor = ShaderGUI.FindProperty("_TintColor", props);

        this.decalTex = ShaderGUI.FindProperty("_DecalTex", props);

        this.dissloveTex = ShaderGUI.FindProperty("_DissloveTex", props);
        this.dissloveAmount = ShaderGUI.FindProperty("_DissloveAmount", props);

        this.uvNoise = ShaderGUI.FindProperty("_UVNoise", props);
        this.uvNoiseBias = ShaderGUI.FindProperty("_UVNoiseBias", props);
        this.uvNoiseIntensity = ShaderGUI.FindProperty("_UVNoiseIntensity", props);
        this.uvNoiseSpeed = ShaderGUI.FindProperty("_UVNoiseSpeed", props);

        this.glowTex = ShaderGUI.FindProperty("_GlowTex", props);
        this.glowSpeed = ShaderGUI.FindProperty("_GlowSpeed", props);
        this.glowColor = ShaderGUI.FindProperty("_GlowColor", props);

        this.rimColor = ShaderGUI.FindProperty("_RimColor", props);
        this.rimIntensity = ShaderGUI.FindProperty("_RimIntensity", props);
        this.rimFresnel = ShaderGUI.FindProperty("_RimFresnel", props);

        this.rimLightColor = ShaderGUI.FindProperty("_RimLightColor", props);
        this.rimLightIntensity = ShaderGUI.FindProperty("_RimLightIntensity", props);
        this.rimLightFresnel = ShaderGUI.FindProperty("_RimLightFresnel", props);
    }

    protected override void OnShaderGUI(MaterialEditor materialEditor, Material[] materials)
    {
        this.BlendModeGUI(materialEditor, materials);
        this.ColorGUI(materialEditor, materials);
        this.FogGUI(materialEditor, materials);
        this.DecalGUI(materialEditor, materials);
        this.DissolveGUI(materialEditor, materials);
        this.UVNoiseGUI(materialEditor, materials);
        this.GlowGUI(materialEditor, materials);
        this.RimGUI(materialEditor, materials);
        this.RimLightGUI(materialEditor, materials);
    }

    private void BlendModeGUI(MaterialEditor materialEditor, Material[] materials)
    {
        var renderingMode = (RenderingMode)this.renderingMode.floatValue;

        {
            EditorGUI.BeginChangeCheck();
            renderingMode = (RenderingMode)EditorGUILayout.Popup("Rendering Mode", (int)renderingMode, BlendNames);
            if (EditorGUI.EndChangeCheck())
            {
                materialEditor.RegisterPropertyChangeUndo("Rendering Mode");
                this.renderingMode.floatValue = (float)renderingMode;
                foreach (var mat in materials)
                {
                    this.UpdateRenderingMode(renderingMode, mat);
                }
            }
        }

        {
            EditorGUI.BeginChangeCheck();
            bool zwriteEnalbed = EditorGUILayout.ToggleLeft(
                "ZWrite", this.zwrite.floatValue != 0.0f);
            if (EditorGUI.EndChangeCheck())
            {
                this.zwrite.floatValue = zwriteEnalbed ? 1.0f : 0.0f;
            }
        }

        {
            if (this.CheckOption(materials, "Enable Alpha Test", "_ALPHA_TEST"))
            {
                materialEditor.RangeProperty(this.cutoff, "Cutoff");
            }
        }

        {
            EditorGUI.BeginChangeCheck();
            int cullMode = (int)this.cullMode.floatValue;
            var cullEnum = (CullMode)EditorGUILayout.EnumPopup(
                "Cull Mode", (CullMode)cullMode);
            if (EditorGUI.EndChangeCheck())
            {
                this.cullMode.floatValue = (float)cullEnum;
            }
        }
    }

    private void ColorGUI(MaterialEditor materialEditor, Material[] materials)
    {
        var options = new GUIContent[] {
            new GUIContent("All"),
            new GUIContent("R"),
            new GUIContent("G"),
            new GUIContent("B"),
            new GUIContent("A")
        };

        var keys = new string[] {
            "_",
            "_CHANNEL_R",
            "_CHANNEL_G",
            "_CHANNEL_B",
            "_CHANNEL_A",
        };

        EditorGUI.BeginChangeCheck();
        int channel = this.ListOptions(materials, options, keys);
        if (channel != 0)
        {
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            materialEditor.TexturePropertySingleLine(new GUIContent("Main Texture"), this.mainTex);
            materialEditor.TextureScaleOffsetProperty(this.mainTex);
            GUILayout.EndVertical();
            if (EditorGUI.EndChangeCheck() || this.mainViewTex == null)
            {
                EditorApplication.delayCall += () =>
                {
                    this.mainViewTex = BuildPreview(this.mainTex, channel);
                    materialEditor.Repaint();
                };
            }

            GUILayout.Box(this.mainViewTex, GUILayout.Width(64), GUILayout.Height(64));
            GUILayout.EndHorizontal();
        }
        else
        {
            EditorGUI.EndChangeCheck();
            materialEditor.TextureProperty(this.mainTex, "Main Texture");
        }

        if (this.CheckOption(materials, "Enable Tint Color", "ENABLE_TINT_COLOR"))
        {
            EditorGUI.indentLevel = 1;
            materialEditor.ColorProperty(this.tintColor, "Tint Color");
            EditorGUI.indentLevel = 0;
        }
    }

    private void FogGUI(MaterialEditor materialEditor, Material[] materials)
    {
        this.CheckOption(materials, "Enable Fog", "ENABLE_FOG");
    }

    private void DecalGUI(MaterialEditor materialEditor, Material[] materials)
    {
        if (!this.CheckOption(materials, "Enable Decal", "ENABLE_DECAL"))
        {
            return;
        }

        EditorGUI.indentLevel = 1;
        var options = new GUIContent[] {
            new GUIContent("All"),
            new GUIContent("R"),
            new GUIContent("G"),
            new GUIContent("B"),
            new GUIContent("A")
        };
        var keys = new string[] {
            "_",
            "_DECAL_CHANNEL_R",
            "_DECAL_CHANNEL_G",
            "_DECAL_CHANNEL_B",
            "_DECAL_CHANNEL_A",
        };

        EditorGUI.BeginChangeCheck();
        int channel = this.ListOptions(materials, options, keys);
        if (channel != 0)
        {
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            materialEditor.TexturePropertySingleLine(new GUIContent("Decal Texture"), this.decalTex);
            materialEditor.TextureScaleOffsetProperty(this.decalTex);
            GUILayout.EndVertical();
            if (EditorGUI.EndChangeCheck() || this.decalViewTex == null)
            {
                EditorApplication.delayCall += () =>
                {
                    this.decalViewTex = BuildPreview(this.decalTex, channel);
                    materialEditor.Repaint();
                };
            }

            GUILayout.Box(this.decalViewTex, GUILayout.Width(64), GUILayout.Height(64));
            GUILayout.EndHorizontal();
        }
        else
        {
            EditorGUI.EndChangeCheck();
            materialEditor.TextureProperty(this.decalTex, "Decal Texture");
        }

        EditorGUI.indentLevel = 0;
    }

    private void DissolveGUI(MaterialEditor materialEditor, Material[] materials)
    {
        if (!this.CheckOption(materials, "Enable Dissolve", "ENABLE_DISSLOVE"))
        {
            return;
        }

        EditorGUI.indentLevel = 1;
        materialEditor.TextureProperty(
            this.dissloveTex, "Dissolve Texture");
        materialEditor.RangeProperty(this.dissloveAmount, "Dissolve Amount");
        EditorGUI.indentLevel = 0;
    }

    private void UVNoiseGUI(MaterialEditor materialEditor, Material[] materials)
    {
        if (this.CheckOption(materials, "Enable UV Noise", "ENABLE_UV_NOISE"))
        {
            EditorGUI.indentLevel = 1;
            materialEditor.TextureProperty(this.uvNoise, "UV Noise");
            materialEditor.FloatProperty(this.uvNoiseBias, "Noise Bias");
            materialEditor.FloatProperty(this.uvNoiseIntensity, "Noise Intensity");
            materialEditor.VectorProperty(this.uvNoiseSpeed, "UV Speed");
            EditorGUI.indentLevel = 0;
        }
    }

    private void GlowGUI(MaterialEditor materialEditor, Material[] materials)
    {
        if (this.CheckOption(materials, "Enable Glow", "ENABLE_GLOW"))
        {
            EditorGUI.indentLevel = 1;
            materialEditor.TextureProperty(this.glowTex, "Glow Texture");
            materialEditor.VectorProperty(this.glowSpeed, "Glow Speed");
            materialEditor.ColorProperty(this.glowColor, "Glow Color");
            EditorGUI.indentLevel = 0;
        }
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
    }

    private void RimLightGUI(MaterialEditor materialEditor, Material[] materials)
    {
        if (this.CheckOption(materials, "Enable Rim Light", "ENABLE_RIM_LIGHT"))
        {
            EditorGUI.indentLevel = 1;
            materialEditor.ColorProperty(this.rimLightColor, "Color");
            materialEditor.RangeProperty(this.rimLightIntensity, "Intensity");
            materialEditor.RangeProperty(this.rimLightFresnel, "Fresnel");
            EditorGUI.indentLevel = 0;
        }
    }

    private void UpdateRenderingMode(RenderingMode renderingMode, Material material)
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
                // 开启alpha_test，则小于culloff的象素，通过clip扔掉。现象是边缘可能有锯齿
                material.EnableKeyword("_ALPHA_TEST");
                // TransparentCutout 是只有透明或不透明两部分，不存在半透明。一般用于镂空
                material.SetOverrideTag("RenderType", "TransparentCutout"); 
                material.renderQueue = -1;
                break;

            case RenderingMode.AlphaBlend:
                // 使用的混合算法:
                // Blend SrcFactor DstFactor: Configure and enable blending.
                // The generated color is multiplied by the SrcFactor. 
                // The color already on screen is multiplied by DstFactor and the two are added together.
                // 使用此算法可以处理成边缘平滑的镂空（因为跟alpha变化有关）

                // 源使用alpha通道中的值
                material.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
                // 目标使用1-源alpha
                material.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
                material.EnableKeyword("_ALPHA_BLEND");
                material.DisableKeyword("_ALPHA_PREMULTIPLY");
                // 有半透明部分
                material.SetOverrideTag("RenderType", "Transparent");
                material.renderQueue = 3000;
                break;

            case RenderingMode.Additive:
                // 源使用alpha通道中的值
                material.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
                // 目标使用1
                material.SetInt("_DstBlend", (int)BlendMode.One);
                material.EnableKeyword("_ALPHA_BLEND");
                material.DisableKeyword("_ALPHA_PREMULTIPLY");
                material.SetOverrideTag("RenderType", "Transparent");
                material.renderQueue = 3000;
                break;
        }
    }

    private static Texture2D BuildPreview(MaterialProperty texProp, int channel)
    {
        Texture2D tex = (Texture2D)texProp.textureValue;
        if (tex == null || channel <= 0 || channel > 4)
        {
            return null;
        }

        var readableTex = BuildReadable(tex);
        return CreateChannelTexture(readableTex, channel);
    }

    private static Texture2D BuildReadable(Texture2D texture)
    {
        var tmp = RenderTexture.GetTemporary(texture.width, texture.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
        Graphics.Blit(texture, tmp);

        RenderTexture previous = RenderTexture.active;
        // All rendering goes into the active RenderTexture. 
        // If the active RenderTexture is null everything is rendered in the main window.
        RenderTexture.active = tmp;
        Texture2D readableTex = new Texture2D(texture.width, texture.height);
        // Read pixels from screen into the saved texture data.
        readableTex.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
        readableTex.Apply();

        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(tmp);

        return readableTex;
    }

    private static Texture2D CreateChannelTexture(Texture2D texture, int channel)
    {
        var gray = new Texture2D(texture.width, texture.height, texture.format, texture.mipmapCount > 0);
        for (int i = 0; i < texture.width; ++i)
        {
            for (int j = 0; j < texture.height; ++j)
            {
                var c = texture.GetPixel(i, j);
                Color c2 = new Color();
                if (1 == channel) { c.g = c.r; c.b = c.r; }
                if (2 == channel) { c.r = c.g; c.b = c.g; }
                if (3 == channel) { c.r = c.b; c.g = c.b; }
                if (4 == channel) { c.r = c.a; c.g = c.a; c.b = c.a; }
                c.a = 1.0f;
                gray.SetPixel(i, j, c);
            }
        }

        gray.Apply();
        return gray;
    }
}
