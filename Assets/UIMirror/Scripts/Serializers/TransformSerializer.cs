using ProtoBuf;
using System.Collections.Generic;
using UnityEngine;


[ProtoContract]
public class TransformData
{
    [ProtoMember(1)]
    public Vector3Data anchoredPosition3D;
    [ProtoMember(2)]
    public QuaternionData localRotationData;
    [ProtoMember(3)]
    public Vector3Data localScale;
    [ProtoMember(4)]
    public Vector2Data sizeDelta;
    [ProtoMember(5)]
    public Vector2Data anchorMin;
    [ProtoMember(6)]
    public Vector2Data anchorMax;
    [ProtoMember(7)]
    public Vector2Data pivot;
}

public class TransformSerializer : MonoBehaviour
{
    private static HashSet<RectTransform> ignoreList = new HashSet<RectTransform>();

    public static void Ignore(RectTransform rectTransform)
    {
        ignoreList.Add(rectTransform);
    }
    public TransformData Serialize(RectTransform rectTransform)
    {
        TransformData transformData = new TransformData();

        transformData.anchoredPosition3D = rectTransform.anchoredPosition3D.Serialize();
        transformData.localRotationData = rectTransform.localRotation.Serialize();
        transformData.localScale = rectTransform.localScale.Serialize();
        transformData.sizeDelta = rectTransform.sizeDelta.Serialize();
        transformData.anchorMin = rectTransform.anchorMin.Serialize();
        transformData.anchorMax = rectTransform.anchorMax.Serialize();
        transformData.pivot = rectTransform.pivot.Serialize();

        return transformData;
    }

    public void Deserialize(TransformData transformData, RectTransform rectTransform)
    {
        if (transformData == null || ignoreList.Contains(rectTransform))
        {
            return;
        }

        rectTransform.anchoredPosition3D = transformData.anchoredPosition3D.Deserialize();
        rectTransform.localRotation = transformData.localRotationData.Deserialize();
        rectTransform.localScale = transformData.localScale.Deserialize();
        rectTransform.sizeDelta = transformData.sizeDelta.Deserialize();
        rectTransform.anchorMin = transformData.anchorMin.Deserialize();
        rectTransform.anchorMax = transformData.anchorMax.Deserialize();
        rectTransform.pivot = transformData.pivot.Deserialize();
    }
}
