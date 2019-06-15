using ProtoBuf;
using UnityEngine;

[ProtoContract]
public struct Vector2Data
{
    [ProtoMember(1)]
    public float x;
    [ProtoMember(2)]
    public float y;
}

[ProtoContract]
public struct Vector3Data
{
    [ProtoMember(1)]
    public float x;
    [ProtoMember(2)]
    public float y;
    [ProtoMember(3)]
    public float z;
}

[ProtoContract]
public struct Vector4Data
{
    [ProtoMember(1)]
    public float x;
    [ProtoMember(2)]
    public float y;
    [ProtoMember(3)]
    public float z;
    [ProtoMember(4)]
    public float w;
}

[ProtoContract]
public struct RectData
{
    [ProtoMember(1)]
    public float x;
    [ProtoMember(2)]
    public float y;
    [ProtoMember(3)]
    public float width;
    [ProtoMember(4)]
    public float height;
}

[ProtoContract]
public struct ColorData
{
    [ProtoMember(1)]
    public float r;
    [ProtoMember(2)]
    public float g;
    [ProtoMember(3)]
    public float b;
    [ProtoMember(4)]
    public float a;
}

[ProtoContract]
public struct QuaternionData
{
    [ProtoMember(1)]
    public float x;
    [ProtoMember(2)]
    public float y;
    [ProtoMember(3)]
    public float z;
    [ProtoMember(4)]
    public float w;
}

[ProtoContract]
public class TextureData
{
    [ProtoMember(1)]
    public int hash;
    [ProtoMember(2)]
    public byte[] data;
    [ProtoMember(3)]
    public int width;
    [ProtoMember(4)]
    public int height;
    [ProtoMember(5)]
    public TextureFormat format;
    [ProtoMember(6)]
    public string name;
}

[ProtoContract]
public class SpriteData
{
    [ProtoMember(1)]
    public TextureData textureData;
    [ProtoMember(2)]
    public RectData rect;
    [ProtoMember(3)]
    public Vector4Data border;
    [ProtoMember(4)]
    public Vector2Data pivot;
    [ProtoMember(5)]
    public float pixelsPerUnit;
    [ProtoMember(6)]
    public uint extrude;
    [ProtoMember(7)]
    public SpriteMeshType meshType;
    [ProtoMember(8)]
    public string name;
}

public static class SerializationExtensions
{
    public static ColorData Serialize(this Color color)
    {
        ColorData colorData = new ColorData
        {
            r = color.r,
            g = color.g,
            b = color.b,
            a = color.a
        };

        return colorData;
    }

    public static Color Deserialize(this ColorData colorData)
    {
        return new Color(colorData.r, colorData.g, colorData.b, colorData.a);
    }

    public static QuaternionData Serialize(this Quaternion q)
    {
        QuaternionData quaternionData = new QuaternionData
        {
            x = q.x,
            y = q.y,
            z = q.z,
            w = q.w
        };
        return quaternionData;
    }

    public static Vector2Data Serialize(this Vector2 v)
    {
        Vector2Data vectorData = new Vector2Data
        {
            x = v.x,
            y = v.y,
        };
        return vectorData;
    }
    public static Vector3Data Serialize(this Vector3 v)
    {
        Vector3Data vectorData = new Vector3Data
        {
            x = v.x,
            y = v.y,
            z = v.z,
        };
        return vectorData;
    }
    public static Vector4Data Serialize(this Vector4 v)
    {
        Vector4Data vectorData = new Vector4Data
        {
            x = v.x,
            y = v.y,
            z = v.z,
            w = v.w
        };
        return vectorData;
    }

    public static RectData Serialize(this Rect r)
    {
        RectData rectData = new RectData
        {
            x = r.x,
            y = r.y,
            width = r.width,
            height = r.height
        };
        return rectData;
    }

    public static Vector2 Deserialize(this Vector2Data vectorData)
    {
        return new Vector2(vectorData.x, vectorData.y);
    }

    public static Vector3 Deserialize(this Vector3Data vectorData)
    {
        return new Vector3(vectorData.x, vectorData.y, vectorData.z);
    }

    public static Vector4 Deserialize(this Vector4Data vectorData)
    {
        return new Vector4(vectorData.x, vectorData.y, vectorData.z, vectorData.w);
    }

    public static Quaternion Deserialize(this QuaternionData quaternionData)
    {
        return new Quaternion(quaternionData.x, quaternionData.y, quaternionData.z, quaternionData.w);
    }

    public static Rect Deserialize(this RectData rectData)
    {
        return new Rect(rectData.x, rectData.y, rectData.width, rectData.height);
    }

    public static TextureData Serialize(this Texture texture)
    {
        TextureData textureData = TextureDataCache.GetTextureData(texture);
        if (textureData == null)
        {
            Texture2D readableTexture = texture.isReadable ? texture as Texture2D : CopyTexture(texture as Texture2D);
            byte[] compressedData = IsCompressed(readableTexture) ? readableTexture.GetRawTextureData() : readableTexture.EncodeToPNG();

            textureData = new TextureData
            {
                hash = texture.GetHashCode(),
                data = compressedData,
                width = readableTexture.width,
                height = readableTexture.height,
                format = readableTexture.format,
                name = texture.name
            };
            TextureDataCache.SetTexureData(texture, textureData);
        }

        return textureData;
    }

    private static bool IsCompressed(Texture2D texture)
    {
        TextureFormat f = texture.format;
        return f == TextureFormat.ETC2_RGB || f == TextureFormat.ETC2_RGBA1 || f == TextureFormat.ETC2_RGBA8 
            || f == TextureFormat.ETC2_RGBA8Crunched || f == TextureFormat.ETC_RGB4 || f == TextureFormat.ETC_RGB4Crunched;
    }

    private static Texture2D CopyTexture(Texture2D source)
    {
        Texture2D copy = new Texture2D(source.width, source.height, TextureFormat.ARGB32, source.mipmapCount > 1);
        RenderTexture rt = RenderTexture.GetTemporary(source.width, source.height);
        Graphics.Blit(source, rt);

        // Copy from RenderTexture to Texture2D
        RenderTexture.active = rt;
        copy.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        copy.Apply();
        RenderTexture.ReleaseTemporary(rt);

        return copy;
    }

    public static Texture2D Deserialize(this TextureData textureData)
    {
        return TextureCache.GetTexture(textureData);
    }

    public static SpriteData Serialize(this Sprite sprite)
    {
        if (sprite)
        {
            return new SpriteData
            {
                pivot = sprite.pivot.Serialize(),
                rect = sprite.rect.Serialize(),
                border = sprite.border.Serialize(),
                pixelsPerUnit = sprite.pixelsPerUnit,
                extrude = 0,
                meshType = SpriteMeshType.FullRect,
                textureData = sprite.texture.Serialize(),
                name = sprite.name
            };
        }
        else
        {
            return null;
        }
    }

    public static Sprite Deserialize(this SpriteData spriteData)
    {
        if (spriteData != null)
        {
            Texture2D texture = spriteData.textureData.Deserialize();
            if (texture != null)
            {
                Sprite sprite = Sprite.Create(texture, spriteData.rect.Deserialize(), spriteData.pivot.Deserialize(), spriteData.pixelsPerUnit, spriteData.extrude, spriteData.meshType, spriteData.border.Deserialize());
                sprite.name = spriteData.name;
                return sprite;
            }
        }
        return null;
    }
}
