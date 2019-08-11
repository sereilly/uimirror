using ProtoBuf;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[ProtoContract]
public class TextData : ElementData<Text>
{
    [ProtoMember(1)]
    public string text;
    [ProtoMember(2)]
    public int fontSize;
    [ProtoMember(3)]
    public string fontName;
    [ProtoMember(4)]
    public ColorData colorData;
    [ProtoMember(5)]
    public TextAnchor alignment;
    [ProtoMember(6)]
    public QuaternionData quaternionData;
}

public class TextSerializer : ILayoutSerializer
{
    public Type LayoutElementType => typeof(Text);
    public Type LayoutDataType => typeof(TextData);

    public ElementDataBase Serialize(UIBehaviour element)
    {
        TextData textData = new TextData();
        Text textElement = element as Text;
        if (textElement)
        {
            textData.text = textElement.text;
            textData.fontSize = textElement.fontSize;
            textData.fontName = textElement.font.name;
            textData.colorData = textElement.color.Serialize();
            textData.alignment = textElement.alignment;
        }
        return textData;
    }

    public void Deserialize(NetworkManager networkManager, ElementDataBase elementData, UIBehaviour element)
    {
        TextData textData = elementData as TextData;
        Text textElement = element as Text;
        if (textElement)
        {
            textElement.text = textData.text;
            textElement.fontSize = textData.fontSize;
            textElement.font = FontCache.GetFont(textData.fontName);
            textElement.color = textData.colorData.Deserialize();
            textElement.alignment = textData.alignment;
        }
    }
}