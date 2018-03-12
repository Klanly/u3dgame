using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Yifan.Core
{
    public static class CameraExtensions
    {
        public static bool SaveRenderTextureAsPNG(this Camera camera, string filename)
        {
            if (!Application.HasProLicense())
            {
                return false;
            }

            var renderTarget = camera.targetTexture;
            if (renderTarget == null)
            {
                return false;
            }

            RenderTexture.active = renderTarget;
            var texture = new Texture2D(
                renderTarget.width,
                renderTarget.height,
                TextureFormat.ARGB32,
                false);

            texture.hideFlags = HideFlags.HideAndDontSave;
            texture.ReadPixels(new Rect(0f, 0f, renderTarget.width, renderTarget.height), 0, 0);
            texture.Apply();
            RenderTexture.active = null;

            try
            {
                byte[] bytes = texture.EncodeToPNG();
                var fileStream = File.OpenWrite(filename);
                fileStream.Write(bytes, 0, bytes.Length);
                fileStream.Close();
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.Message);
                return false;
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(texture);
            }

            return true;
        }
    }
}
