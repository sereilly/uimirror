using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectCache
{
    private static Dictionary<int, GameObject> objectMap = new Dictionary<int, GameObject>();
    private static List<Action> actions = new List<Action>();

    public static void Add(int id, GameObject gameObject)
    {
        objectMap[id] = gameObject;
    }

    public static void Remove(int id)
    {
        GameObject gameObject = objectMap[id];
        objectMap.Remove(id);
    }

    public static int Get(Component monoBehaviour)
    {
        return monoBehaviour.gameObject.GetInstanceID();
    }

    public static GameObject Get(int id)
    {
        return objectMap[id];
    }

    public static void Bind<T>(Action<T> p, int id)
    {
        Action action = () =>
        {
            T comp = Get(id).GetComponent<T>();
            p(comp);
        };
        actions.Add(action);
    }

    public static void BindAll()
    {
        foreach (Action action in actions)
        {
            action();
        }
        actions.Clear();
    }
}
