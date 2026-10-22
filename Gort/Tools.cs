using System.Reflection;

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

    internal static string GetEmbeddedProgram(string programPath)
    {
        try
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"Gort.Programs.{programPath.Trim()}.txt";

            using var stream = assembly.GetManifestResourceStream(resourceName);
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
        catch (Exception ex)
        {
            WriteError($"Error loading program: {ex.Message}");
            return string.Empty;
        }
    }
}
