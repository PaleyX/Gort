namespace Gort;

internal static class Tools
{
    internal static void WriteInfo(string message)
    {
        WriteMessage(message, ConsoleColor.Green);
    }

    internal static void WriteError(string message)
    {
        WriteMessage(message, ConsoleColor.Red);
    }

    private static void WriteMessage(string message, ConsoleColor color = ConsoleColor.White)
    {
        var previous = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ForegroundColor = previous;
    }
}
