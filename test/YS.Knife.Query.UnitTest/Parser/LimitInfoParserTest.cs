using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using YS.Knife.Query.Parser;

namespace YS.Knife.Query.UnitTest.Parser
{
    public class LimitInfoParserTest
    {
        [Fact]
        public void should_return_null_when_parse_empty_string()
        {
            LimitInfo.Parse(null).Should().BeNull();
            LimitInfo.Parse(string.Empty).Should().BeNull();
            LimitInfo.Parse(" \t  ").Should().BeNull();
        }
        [Theory]
        [InlineData("1", "0,1")]
        [InlineData("1,2", "1,2")]
        [InlineData("1 ,2 ", "1,2")]
        public void should_parse_limit_info(string input, string expected)
        {
            // order by 支持函数,不支持常数
            ParseLimitInfoShouldBe(input, expected);
        }
        [Theory]
        [InlineData("1.0")]
        [InlineData("1,2,3")]
        [InlineData("'1','2'")]
        public void should_throw_exception_when_parse_limit_info(string input)
        {
            var action = new Action(() => LimitInfo.Parse(input));
            action.Should().ThrowExactly<ParseException>();

        }
        private void ParseLimitInfoShouldBe(string inputText, string expected)
        {
            LimitInfo.Parse(inputText).ToString().Should().Be(expected);
        }
    }
}
