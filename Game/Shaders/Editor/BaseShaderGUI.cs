using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

class BaseShaderGUI : ShaderGUI
{
    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        this.FindProperties(properties);

        Material[] materials = new Material[materialEditor.targets.Length];
        for (int i = 0; i < materials.Length; ++i)
        {
            materials[i] = materialEditor.targets[i] as Material;
        }

        this.OnShaderGUI(materialEditor, materials);
    }

    protected virtual void FindProperties(MaterialProperty[] props)
    {

    }

    protected virtual void OnShaderGUI(MaterialEditor materialEditor, Material[] materials)
    {

    }

    protected bool HasKeyword(Material[] materials, string key)
    {
        return Array.IndexOf(materials[0].shaderKeywords, key) != -1;
    }

    protected bool CheckOption(Material[] materials, string content, string key)
    {
        var isEnabled = this.HasKeyword(materials, key);

        EditorGUI.BeginChangeCheck();
        isEnabled = EditorGUILayout.ToggleLeft(content, isEnabled);
        if (EditorGUI.EndChangeCheck())
        {
            if (isEnabled)
            {
                foreach (var mat in materials)
                {
                    mat.EnableKeyword(key);
                }
            }
            else
            {
                foreach (var mat in materials)
                {
                    mat.DisableKeyword(key);
                }
            }
        }

        return isEnabled;
    }

    protected int ListOptions(Material[] materials, string[] contents, string[] keys)
    {
        int index = -1;
        foreach (var shaderKey in materials[0].shaderKeywords)
        {
            index = Array.IndexOf(keys, shaderKey);
            if (index >= 0)
            {
                break;
            }
        }

        if (index < 0)
        {
            index = Array.IndexOf(keys, "_");
        }

        EditorGUI.BeginChangeCheck();
        index = EditorGUILayout.Popup(index, contents);

        if (EditorGUI.EndChangeCheck())
        {
            var key = keys[index];
            foreach (var mat in materials)
            {
                foreach (var k in keys)
                {
                    mat.DisableKeyword(k);
                }
            }

            if (key != "_")
            {
                foreach (var mat in materials)
                {
                    mat.EnableKeyword(key);
                }
            }
        }

        return index;
    }

    protected int ListOptions(Material[] materials, GUIContent[] contents, string[] keys)
    {
        int index = -1;
        foreach (var shaderKey in materials[0].shaderKeywords)
        {
            index = Array.IndexOf(keys, shaderKey);
            if (index >= 0)
            {
                break;
            }
        }

        if (index < 0)
        {
            index = Array.IndexOf(keys, "_");
        }

        EditorGUI.BeginChangeCheck();
        index = GUILayout.Toolbar(index, contents);

        if (EditorGUI.EndChangeCheck())
        {
            var key = keys[index];
            foreach (var mat in materials)
            {
                foreach (var k in keys)
                {
                    mat.DisableKeyword(k);
                }
            }

            if (key != "_")
            {
                foreach (var mat in materials)
                {
                    mat.EnableKeyword(key);
                }
            }
        }

        return index;
    }
}
