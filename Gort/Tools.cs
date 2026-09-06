using PaleyExpressions.Runners;

namespace Gort;

internal static class Tools
{
    internal static IRunner GetRunner(string line)
    {
#if USE_EXPRESSION
        return new ExpressionRunner(line, typeof(Functions));
#else
        return new AstRunner(line, typeof(Functions));
#endif
    }

    internal static void WriteInfo(string message)
    {
        WriteMessage(message, ConsoleColor.Green);
    }

    internal static void WriteError(string message)
    {
        WriteMessage(message, ConsoleColor.Red);
    }

    internal static void WriteMessage(string message, ConsoleColor color = ConsoleColor.White)
    {
        var previous = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ForegroundColor = previous;
    }
}
