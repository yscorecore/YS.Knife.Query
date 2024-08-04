using System;
using System.Collections.Generic;
using Xunit;
using static YS.Knife.Query.IntegrationTest.Operators.OperatorTestUtils;


namespace YS.Knife.Query.IntegrationTest.Operators
{
    public class NotEqualsTest
    {

        [Theory]
        [MemberData(nameof(GetTestData))]
        public void ConstantAndConstant(Type leftType, object left, Type rightType, object right, bool result)
        {
            CompareConstantAndConstant(Operator.NotEquals, leftType, left, rightType, right, result);
        }

        [Theory]
        [MemberData(nameof(GetTestData))]
        public void PathAndConstant(Type leftType, object left, Type rightType, object right, bool result)
        {
            ComparePathAndConstant(Operator.NotEquals, leftType, left, rightType, right, result);
        }

        [Theory]
        [MemberData(nameof(GetTestData))]
        public void ConstantAndPath(Type leftType, object left, Type rightType, object right, bool result)
        {
            CompareConstantAndPath(Operator.NotEquals, leftType, left, rightType, right, result);
        }

        [Theory]
        [MemberData(nameof(GetTestData))]
        public void PathAndPath(Type leftType, object left, Type rightType, object right, bool result)
        {
            ComparePathAndPath(Operator.NotEquals, leftType, left, rightType, right, result);
        }

        public static IEnumerable<object[]> GetTestData()
        {
            return new List<object[]>
            {
                new object[]{typeof(int),1, typeof(int),1, false},
                new object[]{typeof(int),1, typeof(int), 2, true},
                new object[]{typeof(int),1, typeof(int?),1, false},
                new object[]{typeof(int),1, typeof(int?), 2, true},
                new object[]{typeof(int),1, typeof(int?), null, true},
                new object[]{typeof(int),1, typeof(double),1.0, false},
                new object[]{typeof(int),1, typeof(double), 2.0, true},

            };
        }


    }
}
