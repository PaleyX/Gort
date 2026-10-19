using GortLang.Commands;

namespace GortLang;

internal class Environment(Action<string> output,Dictionary<string, object?>? variables, Type? functions)
{
    internal Action<string> Output { get; } = output;
    internal readonly Type? Functions = functions;

    internal readonly Dictionary<string, object?> Variables = variables ?? new();
    internal readonly Stack<CommandBase?> Stack = new();

    internal CommandBase? First { get; set; }
    internal CommandBase? Last { get; set; }

    internal T? FindMostRecentInStack<T>() where T : CommandBase
    {
        return Stack.OfType<T>().FirstOrDefault();
    }
}
