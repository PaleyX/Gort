using System.Text.RegularExpressions;

namespace Gort;

internal static partial class Extensions
{
    internal static string StripNumericUnderscores(this string text)
    {
        var result = MyRegex().Replace(text, m =>
        {
            // If it's a quoted string, return as-is
            if (m.Value.StartsWith("\"") || m.Value.StartsWith("'"))
                return m.Value;

            // Otherwise, it's an underscore between digits → remove it
            return "";
        });

        return result; 
    }

    [GeneratedRegex(@"([""']).*?\1|(?<=\d)_(?=\d)")]
    private static partial Regex MyRegex();
}