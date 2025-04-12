using PaleyExpressions;

namespace Gort.Commands;

[Command(':')]
internal class CommandDisplay(int lineNumber) : CommandBase(lineNumber)
{
    private AstRunner _astRunner;

    internal override void Compile(string line)
    {
        _astRunner = Tools.GetAst(line);
    }

    internal override CommandBase? Execute()
    {
        Console.WriteLine(_astRunner.Interpret(Environment.Variables));

        return Next;
    }
}
