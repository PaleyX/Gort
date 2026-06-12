namespace Gort.Commands;

[Command(']')]
internal class CommandEndLoop(int lineNumber) : CommandBase(lineNumber)
{
    private CommandLoop _loop;

    internal override void Compile(string line)
    {
        if (!string.IsNullOrWhiteSpace(line))
        {
            throw new CompileTimeException("Superfluous text on endloop");
        }

        if(Environment.Stack.Peek() is CommandLoop loop)
        {
            _loop = loop;
            _loop.Jump = this;
            _loop.SetBreaks(this);
            Environment.Stack.Pop();
        }
        else
        {
            throw new CompileTimeException("Loop..Endloop mismatch");
        }
    }

    internal override CommandBase? Execute()
    {
        return _loop;
    }
}
