using ProtoBuf.Meta;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIMirrorManager : MonoBehaviour
{
    public static UIMirrorManager Instance { get; private set; }

    private Dictionary<Type, ILayoutSerializer> layoutSerializers = new Dictionary<Type, ILayoutSerializer>();

    public TransformSerializer TransformSerializer;

    protected void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            RegisterSerializers();
        }
        else
        {
            Destroy(this);
        }
    }

    private void RegisterSerializers()
    {
        MetaType edb = RuntimeTypeModel.Default.Add(typeof(ElementDataBase), true);
        var serializers = GetComponentsInChildren<MonoBehaviour>().OfType<ILayoutSerializer>();
        foreach (ILayoutSerializer serializer in serializers)
        {
            Register(serializer, edb);
        }
    }

    public ILayoutSerializer GetLayoutSerializer(Type type)
    {
        ILayoutSerializer layoutSerializer = null;
        layoutSerializers.TryGetValue(type, out layoutSerializer);
        return layoutSerializer;
    }

    public void Register(ILayoutSerializer buttonSerializer, MetaType edb)
    {
        layoutSerializers[buttonSerializer.LayoutElementType] = buttonSerializer;
        edb.AddSubType(layoutSerializers.Count, buttonSerializer.LayoutDataType);
    }
}
