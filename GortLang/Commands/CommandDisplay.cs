using PaleyExpressions.Runners;

namespace GortLang.Commands;

[Command(':')]
internal sealed class CommandDisplay : CommandBase
{
    private IRunner _runner;

    internal CommandDisplay(Environment environment, int lineNumber) : base(environment, lineNumber)
    {
    }

    internal override void Compile(string line)
    {
        _runner = Tools.GetRunner(line, Environment.Functions);
    }

    internal override CommandBase? Execute()
    {
        var result = _runner.Interpret(Environment.Variables);
        Environment.Output(result?.ToString() ?? "nil");

        return Next;
    }
}
