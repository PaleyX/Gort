using Gort.Commands;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Gort;

internal class Program
{
    static void Main(string[] args)
    {
        for (; ; )
        {
            Console.Write("> ");
            var command = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(command))
            {
                continue;
            }

            switch (command[0])
            {
                case '$':
                    GetVariable(command[1..]);
                    break;
                case '&':
                    CompileProgram(command[1..]);
                    if (Environment.First != null)
                    {
                        RunProgram();
                    }
                    break;
                default:
                    var result =Tools.GetAst(command).Interpret(Environment.Variables);
                    Console.WriteLine(result);
                    break;
            }
        }
    }

    private static void GetVariable(string line)
    {
        var pos = line.IndexOf(' ');
        var variable = line[..pos];
        var value = line[(pos + 1)..];
        Environment.Variables[variable] = Tools.GetAst(value).Interpret(Environment.Variables);
    }

    private static void RunProgram()
    {
        var command = Environment.First;

        try
        {
            while (command != null)
            {
                command = command.Execute();
            }
        }
        catch (AssertException ae)
        {
            Console.WriteLine($"Assert on line {command.LineNumber}: {ae.Message}");
        }
    }

    private static void CompileProgram(string programPath)
    {
        Environment.Reset();

        var source = LoadProgram(programPath);
        if (string.IsNullOrWhiteSpace(source))
        {
            return;
        }

        int lineNumber = 0;
        int errors = 0;

        var commandList = CommandBase.GetCommandList();

        try
        {
            var lines = source.Split([System.Environment.NewLine], StringSplitOptions.RemoveEmptyEntries);

            foreach (var rawLine in lines)
            {
                ++lineNumber;

                var line = rawLine.Trim();

                if (string.IsNullOrWhiteSpace(line) || line[0] == '^')
                {
                    continue;
                }

                // special case for unadorned assign
                if (!commandList.Contains(line[0]) && IsIdentifier(line))
                {
                    line = "$" + line;
                }

                var command = CommandBase.GetCommand(line[0], lineNumber);
                if (command == null)
                {
                    Console.WriteLine($"Error: Unknown command '{line[0]}' at line {lineNumber}");
                    ++errors;
                    continue;
                }

                // special case for 'else'
                if(command is CommandIf && line.Trim().Length == 1)
                {
                    command = new CommandElse(lineNumber);
                }

                if(Environment.First == null)
                {
                    Environment.First = command;
                }
                else
                {
                    Environment.Last.Next = command;
                }

                command.Compile(line[1..]);

                Environment.Last = command;
            }

            if(Environment.Stack.Count > 0)
            {
                throw new CompileTimeException($"Error: Unmatched '{Environment.Stack.Peek().GetType().Name}' at line {lineNumber}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error compiling program: {ex.Message} - line {lineNumber}");
            errors++;
        }

        if(errors > 0)
        {
            Console.WriteLine($"Compilation failed with {errors} errors.");
            Environment.Reset();
        }
        else
        {
            Console.WriteLine("Compilation successful.");
        }
    }

    private static string LoadProgram(string programPath)
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
            Console.WriteLine($"Error loading program: {ex.Message}");
            return string.Empty;
        }
    }

    private static bool IsIdentifier(string line)
    {
        var index = line.IndexOf(' ');
        if (index == -1)
        {
            return false;
        }

        var id = line[..index].Trim();

        return Regex.IsMatch(id, Environment.IdentifierPattern);
    }
}
