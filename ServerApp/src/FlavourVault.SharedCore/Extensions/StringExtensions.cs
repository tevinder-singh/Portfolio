namespace FlavourVault.SharedCore.Extensions;
public static class StringExtensions
{
    public static bool IsEmpty(this string value)
    {
        return string.IsNullOrEmpty(value);
    }

    public static bool IsNotEmpty(this string value)
    {
        return !string.IsNullOrEmpty(value);
    }

    public static Guid? ToGuid(this string text)
    {
        if (string.IsNullOrEmpty(text))
            return null;

        if (Guid.TryParse(text, out Guid value))
            return value;

        return null;
    }

    public static string GetValueOrDefault(this string text, string defaultValue)
    {
        return String.IsNullOrEmpty(text) ? defaultValue : text;
    }

}
