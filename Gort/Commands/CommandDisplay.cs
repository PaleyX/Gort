using PaleyExpressions.Runners;

namespace Gort.Commands;

[Command(':')]
internal class CommandDisplay(int lineNumber) : CommandBase(lineNumber)
{
    private IRunner _runner;

    internal override void Compile(string line)
    {
        _runner = Tools.GetRunner(line);
    }

    internal override CommandBase? Execute()
    {
        Console.WriteLine(_runner.Interpret(Environment.Variables));

        return Next;
    }
}
