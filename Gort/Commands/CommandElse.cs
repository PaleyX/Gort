namespace Gort.Commands;

internal class CommandElse(int lineNumber) : CommandBase(lineNumber)
{
    internal override void Compile(string line)
    {
        if (!string.IsNullOrWhiteSpace(line))
        {
            throw new CompileTimeException($"Superfluous text on else. Line {LineNumber}");
        }

        if(Environment.Last is not CommandEndif endif)
        {
            throw new CompileTimeException($"Else mismatch. Line {LineNumber}");
        }

        If = endif.If;

        Environment.Stack.Push(this);
    }
    internal override CommandBase? Execute()
    {
        return Next;
    }

    internal CommandIf? If { get; private set; }
}
