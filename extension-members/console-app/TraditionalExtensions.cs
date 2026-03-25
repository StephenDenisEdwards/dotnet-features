namespace ExtensionMembers.Traditional;

public static class StringExtensions
{
    public static string Truncate(this string s, int maxLength)
        => s.Length <= maxLength ? s : s[..maxLength] + "...";

    public static bool IsNullOrWhitespace(this string? s)
        => string.IsNullOrWhiteSpace(s);
}
