namespace ExtensionMembers.Modern;

public static class StringExtensions
{
    extension(string s)
    {
        public string Truncate(int maxLength)
            => s.Length <= maxLength ? s : s[..maxLength] + "...";
    }

    extension(string? s)
    {
        public bool IsNullOrWhitespace => string.IsNullOrWhiteSpace(s);
    }
}
