using PaleyExpressions;
using System.Text.RegularExpressions;

namespace Gort.Commands;

[Command('$')]
internal class CommandAssign(int lineNumber) : CommandBase(lineNumber)
{
    private string _variableName;
    private AstRunner _astRunner;

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
            _astRunner = Tools.GetAst(line[(index + 1)..].Trim());
        }
        else
        {
            throw new CompileTimeException($"Invalid variable name '{_variableName}'");
        }
    }

    internal override CommandBase? Execute()
    {
        var result = _astRunner.Interpret(Environment.Variables);

        Environment.Variables[_variableName] = result;

        return Next;
    }
}
