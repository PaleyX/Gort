using Gort.Commands;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Gort;

internal class Program
{
    static void Main(string[] args)
    {
        for (; ;) 
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
                    
                    break;
                case '&':
                    var firstCommand = CompileProgram(command[1..]);
                    RunProgram(firstCommand);
                    break;
                default:
                    Console.WriteLine($"Error: Unknown command '{command[0]}'");
                    break;
            }
        }
    }

    private static void RunProgram(CommandBase? command)
    {
        while(command != null)
        {
            command = command.Execute();
        }
    }

    static CommandBase? CompileProgram(string programPath)
    {
        var source = LoadProgram(programPath);
        if (string.IsNullOrWhiteSpace(source))
        {
            return null;
        }

        int lineNumber = 0;
        int errors = 0;

        var commandList = CommandBase.GetCommandList();

        try
        {
            var lines = source.Split([System.Environment.NewLine], StringSplitOptions.RemoveEmptyEntries);

            CommandBase? first = null;
            CommandBase? last = null;

            foreach (var rawLine in lines)
            {
                ++lineNumber;

                var line = rawLine.Trim();

                if (string.IsNullOrWhiteSpace(line) || line[0] == '^')
                {
                    continue;
                }

                // special case for unadorned assign
                if (commandList.Contains(line[0]) == false && IsIdentifier(line))
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

                if(first == null)
                {
                    first = command;
                }
                else
                {
                    last.Next = command;
                }

                last = command;

                command.Compile(line[1..]);
            }

            return first;

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error compiling program: {ex.Message} - line {lineNumber}");
            errors++;
            return null;
        }
    }

    static string LoadProgram(string programPath)
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
