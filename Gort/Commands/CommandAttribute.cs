namespace Gort.Commands;

[AttributeUsage(AttributeTargets.Class)]
internal class CommandAttribute(char sigil) : Attribute
{
    public char Sigil { get; } = sigil;
}
