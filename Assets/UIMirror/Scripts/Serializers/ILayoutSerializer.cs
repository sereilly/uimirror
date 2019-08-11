using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public interface ILayoutSerializer
{
    ElementDataBase Serialize(UIBehaviour element);
    void Deserialize(NetworkManager networkManager, ElementDataBase elementData, UIBehaviour element);

    Type LayoutElementType { get; }
    Type LayoutDataType { get; }
}