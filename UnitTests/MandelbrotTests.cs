using GortLang;

namespace UnitTests;

[Collection("Non-Parallel Collection")]
public class MandelbrotTests
{
    [Fact]
    public void MandelbrotMatches()
    {
        var expected = Tools.GetEmbedded("Mandelbrot");

        using var sw = new StringWriter();
        var savedOut = Console.Out;
        Console.SetOut(sw);
        
        var source = Gort.Tools.GetEmbeddedProgram("Mandelbrot");
        var runner = new Interpreter(source, null, typeof(Gort.Functions));
        runner.Run();

        Assert.Equal(expected, sw.ToString());

        Console.SetOut(savedOut);
    }
}