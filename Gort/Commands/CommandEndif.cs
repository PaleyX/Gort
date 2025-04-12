namespace Gort.Commands;

[Command('}')]
internal class CommandEndif(int lineNumber) : CommandBase(lineNumber)
{
    internal override void Compile(string line)
    {
        if(Environment.Stack.Peek() is CommandIf i)
        {
            i.Jump = this;

            Environment.Stack.Pop();
        }
        else
        {
            throw new CompileTimeException("If..Endif mismatch");
        }
    }
    internal override CommandBase? Execute()
    {
        return Next;
    }
}
