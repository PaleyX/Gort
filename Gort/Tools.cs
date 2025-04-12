using PaleyExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
