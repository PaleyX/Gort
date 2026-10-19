namespace GortLang.Commands;

internal class CommandElse : CommandBase
{
    internal CommandElse(Environment environment, int lineNumber) : base(environment, lineNumber)
    {
    }

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

    internal override CommandBase? Execute() => Next;

    internal CommandIf? If { get; private set; }
}
