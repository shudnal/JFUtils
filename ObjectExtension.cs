using System.Reflection;
using JetBrains.Annotations;

namespace JFUtils;

/// <summary>
///     Helpful Unity Object extensions.
/// </summary>
public static class ObjectExtension
{
    public static string GetObjectString(this object obj, bool includePrivate = false)
    {
        if (obj == null) return "null";

        var output = $"{obj}";
        var type = obj.GetType();
        var publicFields = type.GetFields().Where(f => f.IsPublic);
        foreach (var f in publicFields)
        {
            var value = f.GetValue(obj);
            var valueString = value == null ? "null" : value.ToString();
            output += $"\n {f.Name}: {valueString}";
        }

        var privateFields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (var f in privateFields)
        {
            var value = f.GetValue(obj);
            var valueString = value == null ? "null" : value.ToString();
            output += $"\n[private] {f.Name}: {valueString}";
        }

        var publicProps = type.GetProperties();
        foreach (var f in publicProps)
        {
            var value = f.GetValue(obj, null);
            var valueString = value == null ? "null" : value.ToString();
            output += $"\n {f.Name}: {valueString}";
        }

        return output;
    }

    public static ZNetView? GeZNetView(this MonoBehaviour monoB)
    {
        var value = monoB?.GetType()?.GetField("m_nview", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.GetValue(monoB);
        return value == null ? null : (ZNetView)value;
    }
}