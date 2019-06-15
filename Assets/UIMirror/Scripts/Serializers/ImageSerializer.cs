using ProtoBuf;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[ProtoContract]
public class ImageData : ElementData<Image>
{
    [ProtoMember(1)]
    public ColorData colorData;
    [ProtoMember(2)]
    public SpriteData spriteData;
    [ProtoMember(3)]
    public Image.Type type;
}

public class ImageSerializer : MonoBehaviour, ILayoutSerializer
{
    public Type LayoutElementType => typeof(Image);
    public Type LayoutDataType => typeof(ImageData);

    public ElementDataBase Serialize(UIBehaviour element)
    {
        ImageData imageData = new ImageData();
        Image imageElement = element as Image;
        if (imageElement)
        {
            imageData.colorData = imageElement.color.Serialize();
            imageData.spriteData = imageElement.sprite?.Serialize();
            imageData.type = imageElement.type;
        }
        
        return imageData;
    }

    public void Deserialize(ElementDataBase elementData, UIBehaviour element)
    {
        ImageData imageData = elementData as ImageData;
        Image imageElement = element as Image;
        if (imageElement)
        {
            imageElement.color = imageData.colorData.Deserialize();
            if (imageElement.sprite == null)
            {
                imageElement.sprite = imageData.spriteData.Deserialize();
            }
            imageElement.type = imageData.type;
        }
    }
}