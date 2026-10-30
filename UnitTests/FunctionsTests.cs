using Gort;
using GortLang;

namespace UnitTests;

public class FunctionsTests
{
    [Theory]
    [InlineData("hello", 0d, 2d, "he")]
    [InlineData("hello", 2d, 2d, "ll")]
    [InlineData("hello", 4d, 1d, "o")]
    [InlineData("hello", 5d, 0d, "")]
    public void Substr_ReturnsRequestedRange(string text, double start, double length, string expected)
    {
        Assert.Equal(expected, Functions.Substr(text, start, length));
    }

    [Fact]
    public void Format_FormatsMultipleArguments()
    {
        Assert.Equal("value: 42, ok", Functions.Format("value: {0}, {1}", 42, "ok"));
    }

    [Fact]
    public void Assert_ReturnsTrueForPassingCondition()
    {
        Assert.True(Functions.Assert(true, "not used"));
    }

    [Fact]
    public void Assert_ThrowsAssertExceptionForFailingCondition()
    {
        var exception = Assert.Throws<AssertException>(() => Functions.Assert(false, "failed"));

        Assert.Equal("failed", exception.Message);
    }

    [Theory]
    [InlineData("", "", "")]
    [InlineData("a", "a", "")]
    [InlineData("abc", "a", "bc")]
    public void CarAndCdr_HandleShortAndLongStrings(string text, string expectedCar, string expectedCdr)
    {
        Assert.Equal(expectedCar, Functions.Car(text));
        Assert.Equal(expectedCdr, Functions.Cdr(text));
    }

    [Theory]
    [InlineData("", 0d)]
    [InlineData("hello", 5d)]
    public void Len_ReturnsStringLength(string text, double expected)
    {
        Assert.Equal(expected, Functions.Len(text));
    }

    [Theory]
    [InlineData(0d, 0d)]
    [InlineData(9d, 3d)]
    [InlineData(2.25d, 1.5d)]
    public void Sqrt_ReturnsSquareRoot(double value, double expected)
    {
        Assert.Equal(expected, Functions.Sqrt(value));
    }

    [Fact]
    public void Sqrt_NegativeValueReturnsNaN()
    {
        Assert.True(double.IsNaN(Functions.Sqrt(-1d)));
    }

    [Fact]
    public void Cond_ReturnsFirstMatchingValue()
    {
        var result = Functions.Cond(
            () => false, () => "first",
            () => true, () => "second",
            () => true, () => "third");

        Assert.Equal("second", result);
    }

    [Fact]
    public void Cond_ReturnsNullWhenNothingMatches()
    {
        var result = Functions.Cond(
            () => false, () => "first",
            () => false, () => "second");

        Assert.Null(result);
    }

    [Fact]
    public void Functions_AreCallableThroughInterpreter()
    {
        var variables = new Dictionary<string, object?>();
        var source = string.Join(System.Environment.NewLine,
            "first car(\"hello\")",
            "rest cdr(\"hello\")",
            "size len(\"hello\")",
            "text format(\"{0}!\", first)");
        var runner = new Interpreter(
            source,
            variables,
            typeof(Functions));

        runner.Run();

        Assert.Equal("h", variables["first"]);
        Assert.Equal("ello", variables["rest"]);
        Assert.Equal(5d, variables["size"]);
        Assert.Equal("h!", variables["text"]);
    }
}
