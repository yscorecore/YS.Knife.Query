using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using static YS.Knife.Query.IntegrationTest.Operators.OperatorTestUtils;

namespace YS.Knife.Query.IntegrationTest.Operators
{
    public class ContainsTest
    {
        [Theory]
        [MemberData(nameof(GetTestData))]
        [InlineData(typeof(string), "abc", typeof(string), null, false)]
        [InlineData(typeof(string), null, typeof(string), "abc", false)]
        [InlineData(typeof(string), "123", typeof(int?), null, false)]
        public void ConstantAndConstant(Type leftType, object left, Type rightType, object right, bool result)
        {
            CompareConstantAndConstant(Operator.Contains, leftType, left, rightType, right, result);
        }


        [Theory]
        [MemberData(nameof(GetTestData))]
        public void PathAndConstant(Type leftType, object left, Type rightType, object right, bool result)
        {
            ComparePathAndConstant(Operator.Contains, leftType, left, rightType, right, result);
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
            yield return new(typeof(string), "", typeof(string), "", true);
            yield return new(typeof(string), "abc", typeof(string), "", true);
            yield return new(typeof(string), "abc", typeof(string), "a", true);
            yield return new(typeof(string), "abc", typeof(string), "b", true);
            yield return new(typeof(string), "abc", typeof(string), "c", true);
            yield return new(typeof(string), "abc", typeof(string), "ab", true);
            yield return new(typeof(string), "abc", typeof(string), "bc", true);
            yield return new(typeof(string), "abc", typeof(string), "abc", true);
            yield return new(typeof(string), "abc", typeof(string), "ac", false);
            yield return new(typeof(int), 123, typeof(int), 12, true);
            yield return new(typeof(int), 123, typeof(int), 13, false);
            yield return new(typeof(int), 123, typeof(int?), 12, true);
        }


    }
}
