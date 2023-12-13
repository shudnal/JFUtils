namespace JFUtils;

[PublicAPI]
public static class StringExtension
{
    public static bool IsGood(this string str) { return !string.IsNullOrEmpty(str) && !string.IsNullOrWhiteSpace(str); }

    public static List<string> SplitToList(this string str, string separator = ", ")
    {
        return str.Split(new[] { separator },
            StringSplitOptions.RemoveEmptyEntries).ToList();
    }

    public static string Localize(this string str) { return Localization.instance.Localize(str); }

    public static string Localize(this string text, params string[] args)
    {
        return text.IsGood() ? Localization.instance.Localize(text, args) : string.Empty;
    }

    public static string HumanizeString(this string str)
    {
        string result = "";
        str = str.Replace("$", "");
        bool first = true;
        foreach (char c in str)
        {
            if (!first && char.IsUpper(c))
                result += " " + c.ToString().ToLower();
            else if (c.Equals('_'))
                result += " ";
            else result += c;

            first = false;
        }

        return result;
    }
}