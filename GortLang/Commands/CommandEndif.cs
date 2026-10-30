namespace GortLang.Commands;

[Command('}')]
internal sealed class CommandEndif : CommandBase
{
    internal CommandEndif(Environment environment, int lineNumber) : base(environment, lineNumber)
    {
    }

    internal override void Compile(string line)
    {
        const string mismatch = "If '{' .. '}' Endif mismatch";

        Environment.CheckStack(mismatch);

        var command = Environment.Stack.Pop();

        switch (command)
        {
            case CommandIf i:
                If = i;
                If.Jump = this;
                break;
            case CommandElse e:
                e.If.Jump.Next = this;
                e.If.Jump = e;
                break;
            default:
                throw new CompileTimeException(mismatch);
        }
    }

    internal override CommandBase? Execute() => Next;

    internal CommandIf If { get; private set; }
}
