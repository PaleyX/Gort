using System.Text.RegularExpressions;

namespace GortLang;

internal static partial class Extensions
{
    extension(string text)
    {
        internal string StripNumericUnderscores()
        {
            var result = NumericUnderscoresRegex().Replace(text, m =>
            {
                // If it's a quoted string, return as-is
                if (m.Value.StartsWith("\"") || m.Value.StartsWith("'"))
                    return m.Value;

                // Otherwise, it's an underscore between digits → remove it
                return "";
            });

            return result;
        }

        internal string RemoveComments()
        {
            return CommentRegex().Replace(text, "$1");
        }
    }

    internal static void FormatOutput(this Action<string> output, string text)
    {
        output(text);

        if (output != (Action<string>)Console.WriteLine)
        {
            output(System.Environment.NewLine);
        }
    }

    [GeneratedRegex(@"([""'])(?:\\.|(?!\1).)*\1|(?<=\d)_(?=\d)")]
    private static partial Regex NumericUnderscoresRegex();

    [GeneratedRegex("""^((?:"(?:\\.|[^"\\\r\n])*"|[^"#\r\n])*)#.*$""")]
    private static partial Regex CommentRegex();
}
