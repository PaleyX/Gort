using System.Diagnostics;
using Gort.Commands;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Gort;

public static class Program
{
    private static void Main(string[] args)
    {
        for (; ; )
        {
            Console.Write("> ");
            var command = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(command))
            {
                continue;
            }

            try
            {
                switch (command[0])
                {
                    case '$':
                        GetVariable(command[1..]);
                        break;
                    case '&':
                        if (command.StartsWith("&&"))
                        {
                            var text = GetEmbeddedProgram(command[2..]);
                            RunTimedProgram(text);
                        }
                        else
                        {
                            var text = GetEmbeddedProgram(command[1..]);
                            RunProgram(text);
                        }
                        break;
                    default:
                        var result = Tools.GetRunner(command.StripNumericUnderscores())
                            .Interpret(Environment.Variables);
                        Console.WriteLine(result);
                        break;
                }
            }
            catch (Exception ex)
            {
                Tools.WriteError($"Error: {ex.Message}");
            }
        }
    }

    private static void GetVariable(string line)
    {
        try
        {
            var pos = line.IndexOf(' ');
            var variable = line[..pos];
            var value = line[(pos + 1)..];
            Environment.Variables[variable] = Tools.GetRunner(value).Interpret(Environment.Variables);
        }
        catch (Exception e)
        {
            Tools.WriteError($"Error setting variable: {e.Message}");
        }
    }

    private static void RunTimedProgram(string programText)
    {
        var stopwatch = Stopwatch.StartNew();

        RunProgram(programText);

        stopwatch.Stop();

        // Display elapsed time in various formats
        Tools.WriteInfo($"Elapsed Time: {stopwatch.ElapsedMilliseconds} ms");
        Tools.WriteInfo($"Elapsed Time: {stopwatch.Elapsed.TotalSeconds:F4} seconds");
    }
    
    private static void RunProgram(string programText)
    {
        CommandBase? command = null;

        try
        {
            CompileProgram(programText);

            command = Environment.First;

            while (command != null)
            {
                command = command.Execute();
            }
        }
        catch (AssertException ae)
        {
            Tools.WriteError($"Assert on line {command?.LineNumber.ToString() ?? "<unknown>"}: {ae.Message}");
        }
    }

    private static void CompileProgram(string source)
    {
        Environment.Reset();

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
            Tools.WriteError($"Error compiling program: {ex.Message} - line {lineNumber}");
            errors++;
        }

        if(errors > 0)
        {
            Tools.WriteError($"Compilation failed with {errors} errors.");
            Environment.Reset();
        }
        else
        {
            Tools.WriteInfo("Compilation successful.");
        }
    }

    private static string GetEmbeddedProgram(string programPath)
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
            Tools.WriteError($"Error loading program: {ex.Message}");
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
