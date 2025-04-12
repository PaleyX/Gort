namespace Gort.Commands;

internal abstract class CommandBase(int lineNumber)
{
    internal int LineNumber { get; } = lineNumber;

    internal abstract void Compile(string line);

    internal abstract CommandBase? Execute();

    internal CommandBase? Next { get; set; }

    internal static CommandBase? GetCommand(char command, int lineNumber)
    {
        var type = typeof(CommandBase).Assembly
            .GetTypes()
            .FirstOrDefault(t => t.GetCustomAttributes(typeof(CommandAttribute), false)
                .Any(a => ((CommandAttribute)a).Sigil == command));

        if (type == null)
        {
            return null;
        }

        return (CommandBase?)Activator.CreateInstance(type, lineNumber);
    }

    internal static List<char> GetCommandList()
    {
        var list = new List<char>();
        foreach (var type in typeof(CommandBase).Assembly.GetTypes())
        {
            if (type.GetCustomAttributes(typeof(CommandAttribute), false)
                .FirstOrDefault() is CommandAttribute attribute)
            {
                list.Add(attribute.Sigil);
            }
        }

        return list;
    }
}
