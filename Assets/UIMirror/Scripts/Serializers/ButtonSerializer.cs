using ProtoBuf;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[ProtoContract]
public class ButtonData : ElementData<Button>
{
    [ProtoMember(1)]
    public ColorData normalColor;
    [ProtoMember(2)]
    public string text;
    [ProtoMember(3)]
    public int objectID;
}

public class ButtonSerializer : ILayoutSerializer
{
    public Type LayoutElementType => typeof(Button);
    public Type LayoutDataType => typeof(ButtonData);

    public void Deserialize(NetworkManager networkManager, ElementDataBase elementData, UIBehaviour element)
    {
        ButtonData buttonData = elementData as ButtonData;
        Button buttonElement = element as Button;
        if (buttonElement)
        {
            ColorBlock colorBlock = buttonElement.colors;
            colorBlock.normalColor = buttonData.normalColor.Deserialize();
            buttonElement.colors = colorBlock;

            if (networkManager)
            {
                buttonElement.onClick.RemoveAllListeners();
                buttonElement.onClick.AddListener(() =>
                {
                    InputMessage inputMessage = new InputMessage(InputMessageType.Button, buttonData.objectID);
                    networkManager.SendMessageToServer(inputMessage.Serialize());
                    if (AppSettings.Instance.Vibrate)
                    {
                        Handheld.Vibrate();
                    }
                });
            }
        }
    }

    ElementDataBase ILayoutSerializer.Serialize(UIBehaviour element)
    {
        ButtonData buttonData = new ButtonData();
        Button buttonElement = element as Button;
        if (buttonElement)
        {
            ColorBlock colorBlock = buttonElement.colors;
            buttonData.normalColor = colorBlock.normalColor.Serialize();
            buttonData.objectID = buttonElement.GetInstanceID();
        }
        return buttonData;
    }
}