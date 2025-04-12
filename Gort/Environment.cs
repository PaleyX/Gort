using Gort.Commands;

internal static class Environment 
{
    internal static readonly Dictionary<string, object?> Variables = new();
    internal static readonly Stack<CommandBase?> Stack = new();

    internal const string IdentifierPattern = @"^[a-zA-Z_$][a-zA-Z_$0-9]*$";
}
