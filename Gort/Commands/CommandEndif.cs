namespace Gort.Commands;

[Command('}')]
internal class CommandEndif(int lineNumber) : CommandBase(lineNumber)
{
    internal override void Compile(string line)
    {
        switch (Environment.Stack.Peek())
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

        Environment.Stack.Pop();
    }
    internal override CommandBase? Execute()
    {
        return Next;
    }

    internal CommandIf If { get; private set; }
}
