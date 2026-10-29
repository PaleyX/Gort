namespace GortLang.Commands;

[Command('}')]
internal sealed class CommandEndif : CommandBase
{
    internal CommandEndif(Environment environment, int lineNumber) : base(environment, lineNumber)
    {
    }

    internal override void Compile(string line)
    {
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
                throw new CompileTimeException("If..Endif mismatch");
        }
    }

    internal override CommandBase? Execute() => Next;

    internal CommandIf If { get; private set; }
}
