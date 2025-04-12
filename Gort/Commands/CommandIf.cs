using PaleyExpressions;

namespace Gort.Commands;

[Command('{')]
internal class CommandIf(int lineNumber) : CommandBase(lineNumber)
{
    private AstRunner _astRunner;

    internal override void Compile(string line)
    {
        _astRunner = Tools.GetAst(line);

        Environment.Stack.Push(this);
    }

    internal override CommandBase? Execute()
    {
        var result = _astRunner.Interpret(Environment.Variables);

        if (result is bool b)
        {
            return b ? Next : Jump?.Next;
        }
        else
        {
            throw new RunTimeException($"If guard must evaluate to boolean. Line {LineNumber}");
        }
    }

    internal CommandBase? Jump { get; set; }
}
