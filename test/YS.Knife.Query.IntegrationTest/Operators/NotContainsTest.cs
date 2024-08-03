using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using static YS.Knife.Query.IntegrationTest.Operators.OperatorTestUtils;

namespace YS.Knife.Query.IntegrationTest.Operators
{
    public class NotContainsTest
    {
        [Theory]
        [MemberData(nameof(GetTestData))]
        [InlineData(typeof(string), "abc", typeof(string), null, true)]
        [InlineData(typeof(string), null, typeof(string), "abc", true)]
        [InlineData(typeof(string), "123", typeof(int?), null, true)]
        public void ConstantAndConstant(Type leftType, object left, Type rightType, object right, bool result)
        {
            CompareConstantAndConstant(Operator.NotContains, leftType, left, rightType, right, result);
        }


        [Theory]
        [MemberData(nameof(GetTestData))]
        public void PathAndConstant(Type leftType, object left, Type rightType, object right, bool result)
        {
            ComparePathAndConstant(Operator.NotContains, leftType, left, rightType, right, result);
        }

        public static IEnumerable<object[]> GetTestData()
        {
            return GetTestDataInternal().Select(p => new object[]
             {
                p.Item1,
                p.Item2,
                p.Item3,
                p.Item4,
                p.Item5
             });
        }
        private static IEnumerable<(Type, object, Type, object, bool)> GetTestDataInternal()
        {
            yield return new(typeof(string), "", typeof(string), "", false);
            yield return new(typeof(string), "abc", typeof(string), "", false);
            yield return new(typeof(string), "abc", typeof(string), "a", false);
            yield return new(typeof(string), "abc", typeof(string), "b", false);
            yield return new(typeof(string), "abc", typeof(string), "c", false);
            yield return new(typeof(string), "abc", typeof(string), "ab", false);
            yield return new(typeof(string), "abc", typeof(string), "bc", false);
            yield return new(typeof(string), "abc", typeof(string), "abc", false);
            yield return new(typeof(string), "abc", typeof(string), "ac", true);
            yield return new(typeof(int), 123, typeof(int), 12, false);
            yield return new(typeof(int), 123, typeof(int), 13, true);
            yield return new(typeof(int), 123, typeof(int?), 12, false);
        }


    }
}
