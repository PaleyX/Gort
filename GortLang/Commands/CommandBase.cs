using System.Reflection;

namespace GortLang.Commands;

internal abstract class CommandBase(Environment environment, int lineNumber)
{
    internal int LineNumber { get; } = lineNumber;

    protected readonly Environment Environment = environment;

    internal abstract void Compile(string line);

    internal abstract CommandBase? Execute();

    internal CommandBase? Next { get; set; }

    internal static CommandBase? GetCommand(char command, Environment environment, int lineNumber)
    {
        var type = typeof(CommandBase).Assembly
            .GetTypes()
            .FirstOrDefault(t => t.GetCustomAttributes(typeof(CommandAttribute), false)
                .Any(a => ((CommandAttribute)a).Sigil == command));

        if (type == null)
        {
            return null;
        }

        return (CommandBase?)Activator.CreateInstance(
                type,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                binder: null,
                args: [environment, lineNumber],
                culture: null);
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
