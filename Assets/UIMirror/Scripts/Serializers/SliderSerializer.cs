using ProtoBuf;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[ProtoContract]
public class SliderData : ElementData<Slider>
{
    [ProtoMember(1)]
    public float value;
    [ProtoMember(2)]
    public int targetGraphic;
    [ProtoMember(3)]
    public int fillRect;
    [ProtoMember(4)]
    public int handleRect;
    [ProtoMember(5)]
    public int ID;
}

public class SliderSerializer : MonoBehaviour, ILayoutSerializer
{
    public Type LayoutElementType => typeof(Slider);
    public Type LayoutDataType => typeof(SliderData);

    private bool ignoreServerUpdates;
    private float sentValue;
    private bool ignoreCallback;

    public ElementDataBase Serialize(UIBehaviour element)
    {
        SliderData sliderData = new SliderData();
        Slider sliderElement = element as Slider;
        if (sliderElement)
        {
            sliderData.value = sliderElement.value;
            sliderData.targetGraphic = ObjectCache.Get(sliderElement.targetGraphic);
            sliderData.fillRect = ObjectCache.Get(sliderElement.fillRect);
            sliderData.handleRect = ObjectCache.Get(sliderElement.handleRect);
            sliderData.ID = sliderElement.GetInstanceID();
        }
        
        return sliderData;
    }

    public void Deserialize(ElementDataBase elementData, UIBehaviour element)
    {
        SliderData sliderData = elementData as SliderData;
        Slider sliderElement = element as Slider;
        if (sliderElement)
        {
            if (ignoreServerUpdates)
            {
                if (sentValue == sliderData.value)
                {
                    ignoreServerUpdates = false;
                }
            }
            else
            {
                ignoreCallback = true;
                sliderElement.value = sliderData.value;
                ignoreCallback = false;
                ObjectCache.Bind<Graphic>(x => sliderElement.targetGraphic = x, sliderData.targetGraphic);
                ObjectCache.Bind<RectTransform>(x => sliderElement.fillRect = x, sliderData.fillRect);
                ObjectCache.Bind<RectTransform>(x => sliderElement.handleRect = x, sliderData.handleRect);
            }

            IgnoreTransformUpdates(sliderElement.fillRect);
            IgnoreTransformUpdates(sliderElement.handleRect);

            sliderElement.onValueChanged.AddListener((value) =>
            {
                if (ignoreCallback)
                {
                    return;
                }
                sentValue = value;
                InputMessage inputMessage = new InputMessage(InputMessageType.Slider, sliderData.ID, value);
                NetworkManager.Instance.SendMessageToServer(inputMessage.Serialize());
                ignoreServerUpdates = true;
            });
        }
    }

    private void IgnoreTransformUpdates(RectTransform rectTransform)
    {
        if (rectTransform)
        {
            TransformSerializer.Ignore(rectTransform);
        }
    }
}