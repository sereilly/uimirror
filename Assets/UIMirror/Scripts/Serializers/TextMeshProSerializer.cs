using ProtoBuf;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

[ProtoContract]
public class TextMeshProData : ElementData<TextMeshProUGUI>
{
    [ProtoMember(1)]
    public string text;
    [ProtoMember(2)]
    public float fontSize;
    [ProtoMember(3)]
    public string fontName;
    [ProtoMember(4)]
    public ColorData colorData;
    [ProtoMember(5)]
    public TextAlignmentOptions alignment;
    [ProtoMember(6)]
    public QuaternionData quaternionData;
}

public class TextMeshProSerializer : MonoBehaviour, ILayoutSerializer
{
    public Type LayoutElementType => typeof(TextMeshProUGUI);
    public Type LayoutDataType => typeof(TextMeshProData);

    public ElementDataBase Serialize(UIBehaviour element)
    {
        TextMeshProData textData = new TextMeshProData();
        TextMeshProUGUI textElement = element as TextMeshProUGUI;
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

    public void Deserialize(ElementDataBase elementData, UIBehaviour element)
    {
        TextMeshProData textData = elementData as TextMeshProData;
        TextMeshProUGUI textElement = element as TextMeshProUGUI;
        if (textElement)
        {
            textElement.text = textData.text;
            textElement.fontSize = textData.fontSize;
            textElement.color = textData.colorData.Deserialize();
            textElement.alignment = textData.alignment;
        }
    }
}