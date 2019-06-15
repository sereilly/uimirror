using System.Collections.Generic;
using UnityEngine;

public class TextureCache
{
    private static Dictionary<int, Texture2D> textures = new Dictionary<int, Texture2D>();

    public static Texture2D GetTexture(TextureData textureData)
    {
        if (textures.ContainsKey(textureData.hash))
        {
            return textures[textureData.hash];
        }
        else
        {
            Texture2D texture = null;
            if (textureData.data != null && textureData.data.Length > 0)
            {
                texture = new Texture2D(textureData.width, textureData.height, textureData.format, false, false);
                texture.name = textureData.name;
                texture.LoadImage(textureData.data);
                textures[textureData.hash] = texture;
            }
            return texture;
        }
    }
}
