using PaleyExpressions.Runners;

namespace GortLang.Commands;

[Command(':')]
internal sealed class CommandDisplay : CommandBase
{
    private IRunner _runner;
    private bool _newLine = true;

    internal CommandDisplay(Environment environment, int lineNumber) : base(environment, lineNumber)
    {
    }

    internal override void Compile(string line)
    {
        if(line.FirstOrDefault() == ':')
        {
            _newLine = false;
            line = line[1..];
        }

        _runner = Tools.GetRunner(line, Environment.Functions);
    }

    internal override CommandBase? Execute()
    {
        var result = RunExpression(_runner);
        var output = result?.ToString() ?? "nil";

        if (_newLine)
        {
            Console.WriteLine(output);
        }
        else
        {
            Console.Write(output);
        }

        return Next;
    }
}
