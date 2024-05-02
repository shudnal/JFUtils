#nullable enable
using System.Reflection;

namespace JFUtils;

/// <summary>
///     Helpful Unity Object extensions.
/// </summary>
/// 
[PublicAPI]
public static class ObjectExtension
{
    public static string GetObjectString(this object? obj, bool includePrivate = false)
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

    public static ZNetView? GetZNetView(this MonoBehaviour? monoB)
    {
        var value = monoB?.GetType()
            .GetField("m_nview", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.GetValue(monoB);
        if (value == null) return null;
        return (ZNetView)value;
    }

    public static bool IsCloneOf(this object copy, object original, params string[] ignoreFields)
    {
        var type = copy.GetType();
        var fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
            .Where(x => !ignoreFields.Contains(x.Name))
            .Where(x => x.FieldType.IsValueType).ToList();

        foreach (var fieldInfo in fields)
        {
            var valueOfCopy = fieldInfo.GetValue(copy);
            var valueOfOriginal = fieldInfo.GetValue(original);

            Debug($"{fieldInfo.Name} {valueOfCopy}<->{valueOfOriginal}({valueOfCopy.Equals(valueOfOriginal)})");
            if (!valueOfCopy.Equals(valueOfOriginal)) return false;
        }

        return true;
    }
}