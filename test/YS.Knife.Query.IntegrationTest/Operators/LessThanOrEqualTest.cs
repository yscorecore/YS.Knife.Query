using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using static YS.Knife.Query.IntegrationTest.Operators.OperatorTestUtils;

namespace YS.Knife.Query.IntegrationTest.Operators
{
    public class LessThanOrEqualTest
    {
        [Theory]
        [MemberData(nameof(GetTestData))]
        public void ConstantAndConstant(Type leftType, object left, Type rightType, object right, bool result)
        {
            CompareConstantAndConstant(Operator.LessThanOrEqual, leftType, left, rightType, right, result);
        }


        [Theory]
        [MemberData(nameof(GetTestData))]
        public void PathAndConstant(Type leftType, object left, Type rightType, object right, bool result)
        {
            ComparePathAndConstant(Operator.LessThanOrEqual, leftType, left, rightType, right, result);
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
            yield return new(typeof(int), -1, typeof(int), 0, true);
            yield return new(typeof(int), -1, typeof(int), 1, true);
            yield return new(typeof(int), -1, typeof(int), -1, true);
            yield return new(typeof(int), -1, typeof(int?), 0, true);
            yield return new(typeof(int), -1, typeof(int?), 1, true);
            yield return new(typeof(int), -1, typeof(int?), -1, true);
            yield return new(typeof(int), -1, typeof(int?), null, false);

            yield return new(typeof(int?), -1, typeof(int), 0, true);
            yield return new(typeof(int?), -1, typeof(int), 1, true);
            yield return new(typeof(int?), -1, typeof(int), -1, true);
            yield return new(typeof(int?), null, typeof(int), -1, false);

            yield return new(typeof(int?), -1, typeof(int?), 0, true);
            yield return new(typeof(int?), -1, typeof(int?), 1, true);
            yield return new(typeof(int?), -1, typeof(int?), -1, true);
            yield return new(typeof(int?), null, typeof(int?), -1, false);
            yield return new(typeof(int?), -1, typeof(int?), null, false);
        }


    }
}
