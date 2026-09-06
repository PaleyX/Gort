using PaleyExpressions;

namespace Gort;

internal static class Functions
{
    [Function("substr")]
    public static string Substr(string text, double start, double length)
    {
        return text[(int)start..(int)(start + length)];
    }

    [Function("format")]
    public static string Format(string text, params object?[] args)
    {
        return string.Format(text, args);
    }

    [Function("assert")]
    public static bool Assert(bool test, string text)
    {
        if (!test)
        {
            throw new AssertException(text);
        }

        return test;
    }

    [Function("car")]
    public static string Car(string text)
    {
        return text.Length > 0 ? text[0].ToString() : string.Empty;
    }

    [Function("cdr")]
    public static string Cdr(string text)
    {
        return text.Length > 1 ? text[1..] : string.Empty;
    }

    [Function("len")]
    public static double Len(string text)
    {
        return text.Length;
    }

    [Function("sqrt")]
    public static double Sqrt(double n) => Math.Sqrt(n);

    [Function("cond")]
    public static object? Cond(params Func<object?>[] args)
    {
        for (int i = 0; i < args.Length; i += 2)
        {
            if (args[i]() is true)
            {
                return args[i + 1]();
            }
        }

        return null;
    }
}
