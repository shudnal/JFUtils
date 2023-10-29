using System.ComponentModel;
using System.Reflection;
using UnityEngine.UI;
using Component = UnityEngine.Component;

namespace JFUtils;

public static class GameObjectExtension
{
    public static string GetPrefabName(this GameObject gameObject)
    {
        var prefabName = Utils.GetPrefabName(gameObject);
        for (var i = 0; i < 80; i++) prefabName = prefabName.Replace($" ({i})", "");

        return prefabName;
    }

    public static string GetPrefabName<T>(this T gameObject) where T : Component
    {
        var prefabName = Utils.GetPrefabName(gameObject.gameObject);
        for (var i = 0; i < 80; i++) prefabName = prefabName.Replace($" ({i})", "");

        return prefabName;
    }

    public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component =>
        gameObject.GetComponent<T>() ?? gameObject.AddComponent<T>();

    public static T AddComponentCopy<T>(this GameObject gameObject, T duplicate) where T : Component
    {
        var target = gameObject.AddComponent(duplicate.GetType()) as T;
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