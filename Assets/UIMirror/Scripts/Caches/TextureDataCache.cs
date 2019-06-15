using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureDataCache : MonoBehaviour
{
    private static Dictionary<Texture, TextureData> textureDataMap = new Dictionary<Texture, TextureData>();

    public static TextureData GetTextureData(Texture texture)
    {
        TextureData textureData = null;
        textureDataMap.TryGetValue(texture, out textureData);
        return textureData;
    }

    public static void SetTexureData(Texture texture, TextureData textureData)
    {
        textureDataMap[texture] = textureData;
    }
}
