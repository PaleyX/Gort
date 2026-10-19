using GortLang.Commands;

namespace GortLang;

public class Interpreter(string? source, Dictionary<string, object?> variables, Type? functions, Action<string>? output = null)
{
    private readonly string _source = source ?? throw new ArgumentNullException(nameof(source));
    private readonly Action<string> _output = output ?? Console.WriteLine;

    private int _errors;

    public void Run()
    {
        var environment = new Environment(_output, variables, functions);
        _errors = 0;

        Compile(environment);

        if (_errors > 0)
        {
            throw new CompileTimeException($"Compilation failed with {_errors} errors.");
        }

        RunProgram(environment);
    }

    private static void RunProgram(Environment environment)
    {
        CommandBase? command = null;

        try
        {
            command = environment.First;

            while (command != null)
            {
                command = command.Execute();
            }
        }
        catch (AssertException ae)
        {
            throw new RunTimeException($"Assert on line {command?.LineNumber.ToString() ?? "<unknown>"}: {ae.Message}");
        }
    }


    private void Compile(Environment environment)
    {
        var lineNumber = 0;

        var commandList = CommandBase.GetCommandList();

        try
        {
            var lines = _source.Split([System.Environment.NewLine], StringSplitOptions.RemoveEmptyEntries);

            foreach (var rawLine in lines)
            {
                ++lineNumber;

                var line = rawLine.Trim().StripNumericUnderscores();

                if (string.IsNullOrWhiteSpace(line) || line[0] == '^')
                {
                    continue;
                }

                // special case for unadorned assign
                if (!commandList.Contains(line[0]) && IsIdentifier(line))
                {
                    line = "$" + line;
                }

                var command = CommandBase.GetCommand(line[0], environment, lineNumber);
                if (command == null)
                {
                    Error($"Error: Unknown command '{line[0]}' at line {lineNumber}");
                    continue;
                }

                // special case for 'else'
                if (command is CommandIf && line.Trim().Length == 1)
                {
                    command = new CommandElse(environment, lineNumber);
                }

                if (environment.First == null)
                {
                    environment.First = command;
                }
                else
                {
                    environment.Last.Next = command;
                }

                command.Compile(line[1..]);

                environment.Last = command;
            }

            if (environment.Stack.Count > 0)
            {
                Error($"Error: Unmatched '{environment.Stack.Peek()!.GetType().Name}' at line {lineNumber}");
            }
        }
        catch (Exception ex)
        {
            Error($"Error compiling program: {ex.Message} - line {lineNumber}");
        }
    }

    private void Error(string text)
    {
        _output(text);
        ++_errors;
    }

    private static bool IsIdentifier(string line)
    {
        var index = line.IndexOf(' ');
        if (index == -1)
        {
            return false;
        }

        var id = line[..index].Trim();

        return Tools.IsIdentifier(id);
    }

}