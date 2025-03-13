using System;
using FluentAssertions;
using Xunit;
using YS.Knife.Query.Parser;

namespace YS.Knife.Query.UnitTest.Parser
{
    public class AggInfoParserTest
    {
        [Fact]
        public void should_return_null_when_parse_empty_string()
        {
            AggInfo.Parse(null).Should().BeNull();
            AggInfo.Parse(string.Empty).Should().BeNull();
            AggInfo.Parse(" \t  ").Should().BeNull();
        }
        [Theory]
        [InlineData("a", "a.sum()")]
        [InlineData("a.sum( )", "a.sum()")]
        [InlineData("a.avg( )", "a.avg()")]
        [InlineData("abc", "abc.sum()")]
        [InlineData("a,b,c", "a.sum(),b.sum(),c.sum()")]
        [InlineData(" a , b , c ", "a.sum(),b.sum(),c.sum()")]
        [InlineData("a.b.c", "a.b.c.sum()")]
        [InlineData("a.b.c. sum()", "a.b.c.sum()")]
        [InlineData("a.b.c. avg()", "a.b.c.avg()")]
        [InlineData("a.sum().as(t)", "a.sum().as(t)")]
        [InlineData("a.b.sum().as(t)", "a.b.sum().as(t)")]
        [InlineData("a.b.sum().as(1)", "a.b.sum().as(1)")]
        [InlineData("a.b.sum().as('t')", "a.b.sum().as(t)")]
        [InlineData("a.b.sum().as(\"t\")", "a.b.sum().as(t)")]
        [InlineData("a.sum().as(a1),b,c.avg().as(c3)", "a.sum().as(a1),b.sum(),c.avg().as(c3)")]
        [InlineData("a.sum().as(a1);b;c.avg().as(c3)", "a.sum().as(a1),b.sum(),c.avg().as(c3)")]
        public void should_parse_agg_info(string input, string expected)
        {
            ParseAggInfoShouldBe(input, expected);
        }
        [Theory]
        [InlineData("1")]
        [InlineData("sum()")]
        [InlineData("sum().as()")]
        [InlineData("a.sum2()")]
        [InlineData("a.sum(1)")]
        [InlineData("a.sum().t()")]
        [InlineData("a.sum().as()")]
        [InlineData("a.sum().as(b).as(c)")]
        public void should_throw_exception_when_parse_agg_info(string input)
        {
            var action = new Action(() => AggInfo.Parse(input));
            action.Should().ThrowExactly<ParseException>()
                .WithMessage("Invalid agg item *")
                .WithInnerException<ParseException>();

        }
        private void ParseAggInfoShouldBe(string inputText, string expected)
        {
            AggInfo.Parse(inputText).ToString().Should().Be(expected);
        }

    }
}
