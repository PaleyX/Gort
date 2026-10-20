using System.Diagnostics;
using System.Reflection;
using GortLang;

namespace Gort;

public class Program
{
    private static readonly Dictionary<string, object?> _variables = new();

    private static void Main(string[] args)
    {
        while(true)
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
                        var result = GortLang.Tools.GetRunner(GortLang.Tools.StripNumericUnderscores(command), typeof(Functions))
                            .Interpret(_variables);
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

            if (!GortLang.Tools.IsIdentifier(variable))
            {
                Tools.WriteError($"Error: Invalid variable name '{variable}'");
                return;
            }

            _variables[variable] = GortLang.Tools.GetRunner(value, typeof(Functions)).Interpret(_variables);
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
        var runner = new Interpreter(programText, _variables, typeof(Functions), Console.WriteLine);
        runner.Run();
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
}
