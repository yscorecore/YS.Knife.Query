using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using static YS.Knife.Query.IntegrationTest.Operators.OperatorTestUtils;


namespace YS.Knife.Query.IntegrationTest.Operators
{
    public class NotBetweenTest
    {
        [Theory]
        [MemberData(nameof(GetTestData))]
        public void ConstantAndConstant(Type leftType, object left, Type rightType, object right, bool result)
        {
            CompareConstantAndConstant(Operator.NotBetween, leftType, left, rightType, right, result);
        }


        [Theory]
        [MemberData(nameof(GetTestData))]
        public void PathAndConstant(Type leftType, object left, Type rightType, object right, bool result)
        {
            ComparePathAndConstant(Operator.NotBetween, leftType, left, rightType, right, result);
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
            yield return new(typeof(int), 1, typeof(int[]), null, false);
            yield return new(typeof(int), 1, typeof(object[]), new object[] { 1, null }, false);
            yield return new(typeof(int), 1, typeof(object[]), new object[] { null, 1 }, false);
            yield return new(typeof(int), 1, typeof(object[]), new object[] { 1, 1 }, false);
            yield return new(typeof(int), 1, typeof(int[]), new int[] { 1, 1 }, false);
            yield return new(typeof(int), 1, typeof(int[]), new int[] { 2, 3 }, true);
            yield return new(typeof(int), 1, typeof(int[]), new int[] { -1, 0 }, true);
            yield return new(typeof(int), 1, typeof(int[]), new int[] { 0, 2 }, false);
            yield return new(typeof(int), 1, typeof(int[]), new int[] { 2, 0 }, true);
            yield return new(typeof(string), "1", typeof(object[]), new object[] { "0", "2" }, false);
            yield return new(typeof(DateTime), new DateTime(2024, 7, 25), typeof(object[]), new object[] { new DateTime(2024, 7, 24), new DateTime(2024, 7, 26) }, false);
            yield return new(typeof(DateTime), new DateTime(2024, 7, 25), typeof(DateTime[]), new DateTime[] { new DateTime(2024, 7, 24), new DateTime(2024, 7, 26) }, false);
        }
    }
}
