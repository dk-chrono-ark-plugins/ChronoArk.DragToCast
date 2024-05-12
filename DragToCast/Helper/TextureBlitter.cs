using System.Collections.Generic;
using UnityEngine;

namespace DragToCast.Helper;

#nullable enable

internal static class TextureBlitter
{
    private static readonly Dictionary<string, Texture2D> _cached = [];

    /// <summary>
    /// Blit a new Texture2D from a packed texture atlas with given rect
    /// </summary>
    /// <param name="sourceTexture">packed texture atlas</param>
    /// <param name="textureRect">dst rect</param>
    /// <returns>Unpacked Texture2D from atlas. Result is cached</returns>
    internal static Texture2D Blit(this Texture2D sourceTexture, Rect textureRect)
    {
        var id = sourceTexture.name + textureRect.ToString();
        if (_cached.ContainsKey(id)) {
            return _cached[id];
        }

        if (!sourceTexture.isReadable) {
            var rrTexture = RenderTexture.GetTemporary(
                sourceTexture.width,
                sourceTexture.height,
                0,
                RenderTextureFormat.Default,
                RenderTextureReadWrite.Linear
            );
            Graphics.Blit(sourceTexture, rrTexture);
            var backup = RenderTexture.active;
            RenderTexture.active = rrTexture;
            sourceTexture = new Texture2D(sourceTexture.width, sourceTexture.height);
            sourceTexture.ReadPixels(new(0, 0, rrTexture.width, rrTexture.height), 0, 0);
            sourceTexture.Apply();
            RenderTexture.active = backup;
            RenderTexture.ReleaseTemporary(rrTexture);
        }

        var x = Mathf.FloorToInt(textureRect.x);
        var y = Mathf.FloorToInt(textureRect.y);
        var width = Mathf.FloorToInt(textureRect.width);
        var height = Mathf.FloorToInt(textureRect.height);
        var subTexture = new Texture2D(width, height, TextureFormat.ARGB32, false);

        subTexture.SetPixels(sourceTexture.GetPixels(x, y, width, height));
        subTexture.Apply();
        _cached[id] = subTexture;

        return subTexture;
    }

}
