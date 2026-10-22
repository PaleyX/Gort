using GortLang.Commands;

namespace GortLang;

internal class Environment(Action<string> output, Dictionary<string, object?>? variables, Type? functions)
{
    internal readonly Type? Functions = functions;

    internal readonly Dictionary<string, object?> Variables = variables ?? [];
    internal readonly Stack<CommandBase?> Stack = new();

    internal CommandBase? First { get; set; }
    internal CommandBase? Last { get; set; }

    internal T? FindMostRecentInStack<T>() where T : CommandBase
    {
        return Stack.OfType<T>().FirstOrDefault();
    }

    internal void Output(string text) => output.FormatOutput(text);
}
