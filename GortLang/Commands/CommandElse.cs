namespace GortLang.Commands;

internal sealed class CommandElse : CommandBase
{
    internal CommandElse(Environment environment, int lineNumber) : base(environment, lineNumber)
    {
    }

    internal override void Compile(string line)
    {
        if (!string.IsNullOrWhiteSpace(line))
        {
            throw new CompileTimeException("Superfluous text on else '{'");
        }

        if(Environment.Last is not CommandEndif endif)
        {
            throw new CompileTimeException("An else: '{' must immediately follow an endif: '}'");
        }

        If = endif.If;

        Environment.Stack.Push(this);
    }

    internal override CommandBase? Execute() => Next;

    internal CommandIf? If { get; private set; }
}
