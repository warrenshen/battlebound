using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class JsonList
{
    public static List<T> FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    public static string ToJson<T>(List<T> array, string name)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper).Replace("Items", name);
    }

    public static string ToJson<T>(List<T> array, string name, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint).Replace("Items", name);
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public List<T> Items;
    }
}
