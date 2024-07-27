using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using static YS.Knife.Query.IntegrationTest.Operators.OperatorTestUtils;


namespace YS.Knife.Query.IntegrationTest.Operators
{
    public class BetweenTest
    {
        [Theory]
        [MemberData(nameof(GetTestData))]
        public void ConstantAndConstant(Type leftType, object left, Type rightType, object right, bool result)
        {
            CompareConstantAndConstant(Operator.Between, leftType, left, rightType, right, result);
        }


        [Theory]
        [MemberData(nameof(GetTestData))]
        public void PathAndConstant(Type leftType, object left, Type rightType, object right, bool result)
        {
            ComparePathAndConstant(Operator.Between, leftType, left, rightType, right, result);
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
            yield return new(typeof(int), 1, typeof(int[]), null, true);
            yield return new(typeof(int), 1, typeof(object[]), new object[] { 1, null }, true);
            yield return new(typeof(int), 1, typeof(object[]), new object[] { null, 1 }, true);
            yield return new(typeof(int), 1, typeof(object[]), new object[] { 1, 1 }, true);
            yield return new(typeof(int), 1, typeof(int[]), new int[] { 1, 1 }, true);
            yield return new(typeof(int), 1, typeof(int[]), new int[] { 2, 3 }, false);
            yield return new(typeof(int), 1, typeof(int[]), new int[] { -1, 0 }, false);
            yield return new(typeof(int), 1, typeof(int[]), new int[] { 0, 2 }, true);
            yield return new(typeof(int), 1, typeof(int[]), new int[] { 2, 0 }, false);
            yield return new(typeof(string), "1", typeof(object[]), new object[] { "0", "2" }, true);
            yield return new(typeof(DateTime), new DateTime(2024, 7, 25), typeof(object[]), new object[] { new DateTime(2024, 7, 24), new DateTime(2024, 7, 26) }, true);
            yield return new(typeof(DateTime), new DateTime(2024, 7, 25), typeof(DateTime[]), new DateTime[] { new DateTime(2024, 7, 24), new DateTime(2024, 7, 26) }, true);

        }


    }
}
