using PaleyExpressions.Runners;

namespace GortLang.Commands;

[Command('$')] 
internal class CommandAssign : CommandBase
{
    private string _variableName;

    private IRunner _runner;

    internal CommandAssign(Environment environment, int lineNumber) : base(environment, lineNumber)
    {
    }

    internal override void Compile(string line)
    {
        var index = line.IndexOf(' ');
        if (index == -1)
        {
            throw new CompileTimeException("Bad assign");
        }

        _variableName = line[..index].Trim();

        if (Tools.IsIdentifier(_variableName))
        {
            _runner = Tools.GetRunner(line[(index + 1)..].Trim(), Environment.Functions);
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
