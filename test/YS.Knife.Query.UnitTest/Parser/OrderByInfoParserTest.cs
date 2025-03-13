using System;
using FluentAssertions;
using Xunit;
using YS.Knife.Query.Parser;

namespace YS.Knife.Query.UnitTest.Parser
{
    public class OrderByInfoParserTest
    {
        [Fact]
        public void should_return_null_when_parse_empty_string()
        {
            OrderByInfo.Parse(null).Should().BeNull();
            OrderByInfo.Parse(string.Empty).Should().BeNull();
            OrderByInfo.Parse(" \t  ").Should().BeNull();
        }
        [Theory]
        [InlineData("a", "a.asc()")]
        [InlineData("a,b,c", "a.asc(),b.asc(),c.asc()")]
        [InlineData("a.b.c", "a.b.c.asc()")]
        [InlineData("a.asc()", "a.asc()")]
        [InlineData("a.b.desc()", "a.b.desc()")]
        [InlineData("random()", "random().asc()")]
        [InlineData("random(1).desc( )", "random(1).desc()")]
        [InlineData("date.desc(),random()", "date.desc(),random().asc()")]
        [InlineData("date.desc();random()", "date.desc(),random().asc()")]
        public void should_parse_orderby_info(string input, string expected)
        {
            // order by 支持函数,不支持常数
            ParseOrderByInfoShouldBe(input, expected);
        }
        [Theory]
        [InlineData("1")]
        [InlineData("a.desc().asc()")]
        [InlineData("a.desc().abc()")]
        [InlineData("a.desc().abc")]
        public void should_throw_exception_when_parse_orderby_info(string input)
        {
            var action = new Action(() => OrderByInfo.Parse(input));
            action.Should().ThrowExactly<ParseException>()
                .WithMessage("Invalid orderby item *")
                .WithInnerException<ParseException>();

        }
        private void ParseOrderByInfoShouldBe(string inputText, string expected)
        {
            OrderByInfo.Parse(inputText).ToString().Should().Be(expected);
        }
    }
}
