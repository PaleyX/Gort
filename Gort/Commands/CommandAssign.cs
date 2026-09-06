using PaleyExpressions.Runners;
using System.Text.RegularExpressions;

namespace Gort.Commands;

[Command('$')]
internal class CommandAssign(int lineNumber) : CommandBase(lineNumber)
{
    private string _variableName;

    private IRunner _runner;

    internal override void Compile(string line)
    {
        var index = line.IndexOf(' ');
        if (index == -1)
        {
            throw new CompileTimeException("Bad assign");
        }

        _variableName = line[..index].Trim();

        if (Regex.IsMatch(_variableName, Environment.IdentifierPattern))
        {
            _runner = Tools.GetRunner(line[(index + 1)..].Trim());
        }
        else
        {
            throw new CompileTimeException($"Invalid variable name '{_variableName}'");
        }
    }

    internal override CommandBase? Execute()
    {
        var result = _runner.Interpret(Environment.Variables);

        Environment.Variables[_variableName] = result;

        return Next;
    }
}
