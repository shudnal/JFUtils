using System.ComponentModel;
using System.Reflection;
using UnityEngine.UI;
using Component = UnityEngine.Component;

namespace JFUtils;

[PublicAPI]
public static class GameObjectExtension
{
    public static string GetPrefabName(this Object obj)
    {
        var prefabName = Utils.GetPrefabName(obj.name);
        for (var i = 0; i < 80; i++) prefabName = prefabName.Replace($" ({i})", "");

        return prefabName;
    }

    public static string GetPrefabName<T>(this T obj) where T : Object
    {
        var prefabName = Utils.GetPrefabName(obj.name);
        for (var i = 0; i < 80; i++) prefabName = prefabName.Replace($" ({i})", "");

        return prefabName;
    }

    public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component =>
        gameObject.GetComponent<T>() ?? gameObject.AddComponent<T>();

    public static Component GetOrAddComponent(this GameObject gameObject, Type type) =>
        gameObject.GetComponent(type) ?? gameObject.AddComponent(type);

    public static T AddComponentCopy<T>(this GameObject gameObject, T duplicate) where T : Component
    {
        if (duplicate?.GetType() is null)
        {
            DebugError("AddComponentCopy: duplicate is null");
            return default;
        }

        var target = gameObject.GetOrAddComponent(duplicate.GetType()) as T;
        const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        foreach (var propertyInfo in duplicate.GetType().GetProperties(flags))
        {
            if (propertyInfo.Name == "rayTracingMode") continue;

            if (propertyInfo.CanWrite && propertyInfo.GetMethod != null)
                propertyInfo.SetValue(target, propertyInfo.GetValue(duplicate));
        }

        foreach (var fieldInfo in duplicate.GetType().GetFields(flags))
        {
            if (fieldInfo.Name == "rayTracingMode") continue;
            
            fieldInfo.SetValue(target, fieldInfo.GetValue(duplicate));
        }

        return target;
    }
}