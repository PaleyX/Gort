using PaleyExpressions;

namespace Gort;

internal static class Tools
{
    internal static AstRunner GetAst(string line)
    {
        var astRunner = new AstRunner(line, typeof(Functions));
        astRunner.GetAst();

        return astRunner;
    }
}
