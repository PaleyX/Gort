using Gort;

namespace UnitTests;

public class ExtensionsTests
{
    [Theory]
    [InlineData("1_0", "10")]
    [InlineData("_12", "_12")]
    [InlineData("12_", "12_")]
    [InlineData("1_2_3", "123")]
    [InlineData("_1_20_30_4_", "_120304_")]
    [InlineData("1000_000_000", "1000000000")]
    public void SimpleUnderscoreStripsWork(string text, string expected)
    {
        var result = text.StripNumericUnderscores();

        Assert.Equal(expected, result);
    }

    [Fact]
    public void QuotedNumberIsNotStrippedOfUnderscores()
    {
        const string original = "This shouldn't be stripped \"1000_000_000\" - ok";

        Assert.Equal(original, original.StripNumericUnderscores());
    }
}