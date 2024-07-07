using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace YS.Knife.Query.IntegrationTest
{
    public class FilterExtensionsTest
    {
        [Theory]
        [InlineData(1, 1)]
        public void ConstantEqualsTest(object left, object right)
        {
            var leftValue = new ValueInfo
            {
                IsConstant = true,
                ConstantValue = left
            };
            var rightValue = new ValueInfo
            {
                IsConstant = true,
                ConstantValue = right
            };
            var filter = new FilterInfo
            {
                OpType = CombinSymbol.SingleItem,
                Left = leftValue,
                Right = rightValue,
                Operator = Operator.Equals
            };
            var source = new int[] { 1 };
            var target = source.AsQueryable().DoFilter(filter).ToArray();
            target.Should().BeEquivalentTo(source);
        }
    }
}
