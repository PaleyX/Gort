using Gort.Commands;

namespace Gort;

internal static class Environment
{
    internal static readonly Dictionary<string, object?> Variables = new();
    internal static readonly Stack<CommandBase?> Stack = new();

    internal const string IdentifierPattern = "^[a-zA-Z_$][a-zA-Z_$0-9]*$";

    internal static CommandBase? First { get; set; }
    internal static CommandBase? Last { get; set; }

    internal static T? FindMostRecentInStack<T>() where T : CommandBase
    {
        return Stack.OfType<T>().FirstOrDefault();
    }

    internal static void Reset()
    {
        Stack.Clear();

        First = null;
        Last = null;
    }
}
