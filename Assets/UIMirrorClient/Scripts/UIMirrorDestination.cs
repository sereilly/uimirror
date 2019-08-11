using ProtoBuf;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class Node
{
    public GameObject gameObject;
    public Dictionary<int, Node> children = new Dictionary<int, Node>();
    public int ID;
    public bool IsValid = true;
}

public class UIMirrorDestination : MonoBehaviour
{
    [SerializeField]
    protected NetworkManager networkManager;

    private Node root;

    protected void Awake()
    {
        Clear();
    }

    public GameObject Deserialize(byte[] bytes)
    {
        ObjectData objectData;
        using (MemoryStream ms = new MemoryStream(bytes))
        {
            objectData = Serializer.Deserialize<ObjectData>(ms);
        }
        root.ID = objectData.ID;
        root = Deserialize(objectData, root, null);
        ObjectCache.BindAll();
        return root.gameObject;
    }

    private Node Deserialize(ObjectData objectData, Node node, Transform parent)
    {
        // node exists but it doesn't match the current object, destroy it.
        if (node != null && objectData.ID != node.ID)
        {
            Destroy(node?.gameObject);
        }

        // No object? Destroy corresponding node game object if it exists
        if (objectData == null)
        {
            return null;
        }

        if (node == null)
        {
            node = new Node();
            node.ID = objectData.ID;
            node.gameObject = new GameObject();
            node.gameObject.AddComponent<RectTransform>();
            node.gameObject.transform.SetParent(parent);
            node.gameObject.name = objectData.Name;
        }
        
        
        List<ElementDataBase> elementsData = objectData.elementsData;

        foreach (ElementDataBase elementData in elementsData)
        {
            Component layoutElement = elementData.GetLayoutComponentFromObject(node.gameObject);
            if (!layoutElement)
            {
                layoutElement = elementData.AddLayoutComponentToObject(node.gameObject);
            }

            if (layoutElement)
            {
                ILayoutSerializer serializer = UIMirrorManager.Instance.GetLayoutSerializer(layoutElement.GetType());
                serializer.Deserialize(networkManager, elementData, layoutElement as UIBehaviour);
            }
        }

        UIMirrorManager.Instance.TransformSerializer.Deserialize(objectData.transformData, node.gameObject.GetComponent<RectTransform>());

        foreach (Node childNode in node.children.Values)
        {
            childNode.IsValid = false;
        }

        foreach (ObjectData childData in objectData.childrenData)
        {
            int id = childData.ID;
            node.children.TryGetValue(id, out Node childNode);
            childNode = Deserialize(childData, childNode, node.gameObject.transform);
            if (childNode != null)
            {
                node.children[id] = childNode;
                ObjectCache.Add(id, childNode.gameObject);
                childNode.IsValid = true;
            }
        }

        List<Node> children = node.children.Values.ToList();
        foreach (Node child in children)
        {
            if (!child.IsValid)
            {
                Destroy(child.gameObject);
                node.children.Remove(child.ID);
                ObjectCache.Remove(child.ID);
            }
        }

        return node;
    }

    public void Clear()
    {
        Clear(root);

        root = new Node();
        root.gameObject = gameObject;
    }

    private void Clear(Node node)
    {
        if (node == null)
        {
            return;
        }

        foreach (Node childNode in node.children.Values)
        {
            Clear(childNode);
            Destroy(childNode.gameObject);
        }
    }
}
