using ProtoBuf;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if !UNITY_EDITOR && (UNITY_WSA || UNITY_STANDALONE)
using UnityEngine.XR;
#endif

[ProtoContract]
public abstract class ElementDataBase
{
    public abstract Component AddLayoutComponentToObject(GameObject gameObject);
    public abstract Component GetLayoutComponentFromObject(GameObject gameObject);
}

[Serializable]
public class ElementData<T> : ElementDataBase where T : Component
{
    public override Component AddLayoutComponentToObject(GameObject gameObject)
    {
        Component comp = gameObject.GetComponent<T>();
        if (!comp)
        {
            comp = gameObject.AddComponent<T>();
        }
        return comp;
    }

    public override Component GetLayoutComponentFromObject(GameObject gameObject)
    {
        return gameObject.GetComponent<T>();
    }
}

[ProtoContract]
public class ObjectData
{
    [ProtoMember(1)]
    public List<ElementDataBase> elementsData = new List<ElementDataBase>();
    [ProtoMember(2)]
    public TransformData transformData;
    [ProtoMember(3)]
    public List<ObjectData> childrenData = new List<ObjectData>();
    [ProtoMember(4)]
    public string Name;
    [ProtoMember(5)]
    public int ID;
}

public class UIMirrorSource : MonoBehaviour
{
    [SerializeField]
    protected Canvas canvas;
    private ConcurrentQueue<ObjectData> objectDataQueue = new ConcurrentQueue<ObjectData>();
    AutoResetEvent waitHandle = new AutoResetEvent(false);

    protected void Awake()
    {
        InitCanvas();
    }

    private void InitCanvas()
    {
        if (canvas == null)
        {
            canvas = GetComponent<Canvas>();
        }

        if (canvas == null)
        {
            canvas = FindObjectOfType<Canvas>();
        }

        if (canvas == null)
        {
            Debug.LogError("No canvas found");
        }
#if !UNITY_EDITOR && (UNITY_WSA || UNITY_STANDALONE)
        else if (XRSettings.isDeviceActive)
        {
            // Hide canvas when running on device
            canvas.enabled = false;
        }
#endif
    }

    public void Serialize(ConcurrentQueue<byte[]> queue)
    {
        if (canvas != null)
        {
            ObjectData objectData = new ObjectData();
            Serialize(objectData, canvas.gameObject, true);
            objectDataQueue.Enqueue(objectData);
            waitHandle.Set();
        }
    }

    public void SerializeTask(ConcurrentQueue<byte[]> queue)
    {
        try
        {
            while (true)
            {
                waitHandle.WaitOne();
                ObjectData objectData;
                while (objectDataQueue.TryDequeue(out objectData))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        Serializer.Serialize(ms, objectData);
                        queue.Enqueue(ms.ToArray());
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }


    public void InvokeButton(int id)
    {
        Button button = GetComponentWithId<Button>(id);
        if (button)
        {
            button.onClick.Invoke();
        }
    }

    public void MoveSlider(int id, float value)
    {
        Slider slider = GetComponentWithId<Slider>(id);
        if (slider)
        {
            slider.value = value;
        }
    }

    private T GetComponentWithId<T>(int id) where T : Component
    {
        return canvas.GetComponentsInChildren<T>().FirstOrDefault(b => b.GetInstanceID() == id);
    }

    private void Serialize(ObjectData objectData, GameObject go, bool isRoot)
    {
        UIBehaviour[] layoutElements = go.GetComponents<UIBehaviour>();
        objectData.Name = go.name;
        objectData.ID = go.GetInstanceID();

        foreach (UIBehaviour layoutElement in layoutElements)
        {
            ILayoutSerializer layoutSerializer = UIMirrorManager.Instance.GetLayoutSerializer(layoutElement.GetType());
            if (layoutSerializer != null)
            {
                ElementDataBase elementData = layoutSerializer.Serialize(layoutElement as UIBehaviour);
                objectData.elementsData.Add(elementData);

            }
            else if (layoutElement.GetType() != typeof(CanvasScaler) && layoutElement.GetType() != typeof(GraphicRaycaster))
            {
                Debug.LogWarningFormat("No matching ILayoutSerializer for type {0}", layoutElement.GetType());
            }
        }

        if (!isRoot)
        {
            objectData.transformData = UIMirrorManager.Instance.TransformSerializer.Serialize(go.GetComponent<RectTransform>());
        }

        foreach (Transform child in go.transform)
        {
            ObjectData childData = new ObjectData();
            Serialize(childData, child.gameObject, false);
            objectData.childrenData.Add(childData);
        }
    }
}
