using GortLang;

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
    [InlineData("1_000.50", "1000.50")]
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

    [Theory]
    [InlineData("'1000_000'", "'1000_000'")]
    [InlineData("\"escaped \\\"1000_000\\\"\"", "\"escaped \\\"1000_000\\\"\"")]
    public void QuotedNumbersKeepUnderscores(string text, string expected)
    {
        Assert.Equal(expected, text.StripNumericUnderscores());
    }

    [Theory]
    [InlineData("# start of line", "")]
    [InlineData("middle of# line", "middle of")]
    [InlineData("end of line #", "end of line ")]
    [InlineData("multiple # comments # in # line", "multiple ")]
    [InlineData("no comments here", "no comments here")]
    [InlineData("a comment \"in a # string\"", "a comment \"in a # string\"")]
    [InlineData("a comment \"escaped \\\"quote\\\" # string\" # end", "a comment \"escaped \\\"quote\\\" # string\" ")]
    public void RemoveCommentsWorks(string text, string expected)
    {
        var result = text.RemoveComments();

        Assert.Equal(expected, result);
    }
}