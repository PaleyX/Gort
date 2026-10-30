namespace GortLang.Commands;

[Command(']')]
internal sealed class CommandEndLoop : CommandBase
{
    internal CommandEndLoop(Environment environment, int lineNumber) : base(environment, lineNumber)
    {
    }

    private CommandLoop _loop;

    internal override void Compile(string line)
    {
        if (!string.IsNullOrWhiteSpace(line))
        {
            throw new CompileTimeException("Superfluous text on endloop");
        }

        const string mismatch = "Loop '[' .. ']' Endloop mismatch";

        Environment.CheckStack(mismatch);

        if (Environment.Stack.Pop() is CommandLoop loop)
        {
            _loop = loop;
            _loop.Jump = this;
            _loop.SetBreaks(this);
        }
        else
        {
            throw new CompileTimeException(mismatch);
        }
    }

    internal override CommandBase? Execute() => _loop;
}
