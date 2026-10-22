#define USE_EXPRESSION

using PaleyExpressions.Runners;
using System.Text.RegularExpressions;
using GortLang.Commands;

namespace GortLang;

public static class Tools
{
    private const string IdentifierPattern = "^[a-zA-Z_$][a-zA-Z_$0-9]*$";

    public static IRunner GetRunner(string line, Type? functions)
    {
#if USE_EXPRESSION
        return new ExpressionRunner(line, functions);
#else
            return new AstRunner(line, functions);
#endif
    }

    public static string StripNumericUnderscores(string text)
    {
        return text.StripNumericUnderscores();
    }

    public static bool IsIdentifier(string text)
    {
        return Regex.IsMatch(text, IdentifierPattern);
    }

    internal static char GetSigil(Type type)
    {
        // get CommandAttribute from type
        var attribute = (CommandAttribute?)Attribute.GetCustomAttribute(type, typeof(CommandAttribute));

        return attribute?.Sigil ?? 'X';
    }
}