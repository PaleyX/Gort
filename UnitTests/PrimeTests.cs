using GortLang;
using System.Text;

namespace UnitTests;

public class PrimeTests
{
    [Fact]
    public void PrimeProgramWorks()
    {
        var sb = new StringBuilder();

        // run the Primes program, outputting to the StringBuilder
        var source = Gort.Tools.GetEmbeddedProgram("Primes");
        var runner = new Interpreter(source, null, typeof(Gort.Functions), t => sb.Append(t));
        runner.Run();

        // C# Primes 
        var sbC = new StringBuilder();
        for(var i = 1; i <= 10_000; i++)
        {
            if (IsPrime(i))
            {
                sbC.Append(i)
                    .Append(" is Prime")
                    .Append(System.Environment.NewLine);
            }
        }

        Assert.Equal(sbC.ToString(), sb.ToString());
    }

    private static bool IsPrime(int num)
    {
        switch (num)
        {
            case <= 1:
                return false; // 0, 1, and negatives are not prime
            case 2:
                return true; // 2 is prime
        }

        if (num % 2 == 0) return false; // eliminate even numbers > 2

        var boundary = (int)Math.Floor(Math.Sqrt(num));

        for (var i = 3; i <= boundary; i += 2)
        {
            if (num % i == 0)
                return false;
        }
        return true;
    }
}