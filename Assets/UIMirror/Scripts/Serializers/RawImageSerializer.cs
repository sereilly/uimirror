using ProtoBuf;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[ProtoContract]
public class RawImageData : ElementData<RawImage>
{
    [ProtoMember(1)]
    public ColorData colorData;
    [ProtoMember(2)]
    public TextureData textureData;
    [ProtoMember(3)]
    public RectData uvRect;
}

public class RawImageSerializer : MonoBehaviour, ILayoutSerializer
{
    public Type LayoutElementType => typeof(RawImage);
    public Type LayoutDataType => typeof(RawImageData);

    public ElementDataBase Serialize(UIBehaviour element)
    {
        RawImageData imageData = new RawImageData();
        RawImage imageElement = element as RawImage;
        if (imageElement)
        {
            imageData.colorData = imageElement.color.Serialize();
            imageData.textureData = imageElement.texture?.Serialize();
            imageData.uvRect = imageElement.uvRect.Serialize();
        }
        
        return imageData;
    }

    public void Deserialize(ElementDataBase elementData, UIBehaviour element)
    {
        RawImageData imageData = elementData as RawImageData;
        RawImage imageElement = element as RawImage;
        if (imageElement)
        {
            imageElement.color = imageData.colorData.Deserialize();
            if (imageElement.texture == null)
            {
                imageElement.texture = imageData.textureData.Deserialize();
            }
            imageElement.uvRect = imageData.uvRect.Deserialize();
        }
    }
}