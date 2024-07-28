using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;
using static YS.Knife.Query.IntegrationTest.Operators.OperatorTestUtils;


namespace YS.Knife.Query.IntegrationTest.Operators
{
    public class EqualsTest
    {

        [Theory]
        [MemberData(nameof(GetTestData))]
        [MemberData(nameof(GetConstantAndConstantTestData))]
        public void ConstantAndConstant(Type leftType, object left, Type rightType, object right, bool result)
        {
            CompareConstantAndConstant(Operator.Equals, leftType, left, rightType, right, result);
        }
        public static IEnumerable<object[]> GetConstantAndConstantTestData()
        {
            return new List<object[]>
            {
                new object[]{ typeof(DateTime),new DateTime(2024,7,8), typeof(string), "2024-07-08",true},
                new object[]{ typeof(DateTime),new DateTime(2024,7,8), typeof(string), "2024-07-08 00:00:00",true},
                new object[]{ typeof(DateTimeOffset),new DateTimeOffset(new DateTime(2024, 7, 8)), typeof(string), "2024-07-08",true},
                new object[]{ typeof(DateTimeOffset),new DateTimeOffset(new DateTime(2024, 7, 8)), typeof(string), "2024-07-08 00:00:00",true},
                new object[]{ typeof(Guid),new Guid("C7BD06E4-DFFB-4110-860C-9DC36523E9A9"), typeof(string), "c7bd06e4-dffb-4110-860c-9dc36523e9a9", true},
                new object[]{ typeof(string),"1", typeof(object), null, false },
                new object[]{ typeof(int),1, typeof(object),null, false },
                new object[]{ typeof(string),"1", typeof(object), null, false },

            };
        }



        [Theory]
        [MemberData(nameof(GetTestData))]
        public void PathAndConstant(Type leftType, object left, Type rightType, object right, bool result)
        {
            ComparePathAndConstant(Operator.Equals, leftType, left, rightType, right, result);
        }

        [Theory]
        [MemberData(nameof(GetTestData))]
        public void ConstantAndPath(Type leftType, object left, Type rightType, object right, bool result)
        {
            CompareConstantAndPath(Operator.Equals, leftType, left, rightType, right, result);
        }

        [Theory]
        [MemberData(nameof(GetTestData))]
        public void PathAndPath(Type leftType, object left, Type rightType, object right, bool result)
        {
            ComparePathAndPath(Operator.Equals, leftType, left, rightType, right, result);
        }

        public static IEnumerable<object[]> GetTestData()
        {
            return new List<object[]>
            {
                new object[]{typeof(int),1, typeof(int),1, true},
                new object[]{typeof(int),1, typeof(int), 2, false},
                new object[]{typeof(int),1, typeof(short), (short)1,true},
                new object[]{typeof(int),1, typeof(ushort), (ushort)1, true },
                new object[]{typeof(int),1, typeof(uint), (uint)1, true },
                new object[]{typeof(int),1, typeof(ulong), (ulong)1, true },
                new object[]{typeof(int),1, typeof(long), 1L, true },
                new object[]{typeof(int),1, typeof(double),1.0, true },
                new object[]{typeof(int),1, typeof(decimal),1.0M, true },
                new object[]{typeof(int),1, typeof(string), "1", true },
                new object[]{ typeof(long),1L,typeof(int), 1, true},
                new object[]{ typeof(long),1L, typeof(short), (short)1,true},
                new object[]{ typeof(long),1L, typeof(ushort),(ushort)1, true },
                new object[]{ typeof(long),1L, typeof(uint), (uint)1, true },
                new object[]{ typeof(long),1L, typeof(ulong), (ulong)1, true },
                new object[]{ typeof(long),1L, typeof(long), 1L, true },
                new object[]{ typeof(long),1L, typeof(double), 1.0, true },
                new object[]{ typeof(long),1L, typeof(decimal),1.0M, true },
                new object[]{ typeof(long),1L, typeof(string), "1", true },
                new object[]{ typeof(double),1.0, typeof(int), 1, true},
                new object[]{ typeof(double),1.0, typeof(short), (short)1,true},
                new object[]{ typeof(double),1.0, typeof(ushort),(ushort)1, true },
                new object[]{ typeof(double),1.0, typeof(uint),(uint)1, true },
                new object[]{ typeof(double),1.0, typeof(ulong), (ulong)1, true },
                new object[]{ typeof(double),1.0, typeof(long), 1L, true },
                new object[]{ typeof(double),1.0, typeof(double), 1.0, true },
                new object[]{ typeof(double),1.0, typeof(decimal), 1.0M, true },
                new object[]{ typeof(double),1.0, typeof(string), "1", true },
                new object[]{ typeof(decimal),1M, typeof(int), 1, true},
                new object[]{ typeof(decimal),1M, typeof(short), (short)1,true},
                new object[]{ typeof(decimal),1M, typeof(ushort), (ushort)1, true },
                new object[]{ typeof(decimal),1M, typeof(uint), (uint)1, true },
                new object[]{ typeof(decimal),1M, typeof(ulong), (ulong)1, true },
                new object[]{ typeof(decimal),1M, typeof(long), 1L, true },
                new object[]{ typeof(decimal),1M, typeof(double), 1.0, true },
                new object[]{ typeof(decimal),1M, typeof(decimal), 1.0M, true },
                new object[]{ typeof(decimal),1M, typeof(string), "1", true },
                new object[]{ typeof(string),"1", typeof(int), 1, true},
                new object[]{ typeof(string),"1", typeof(short), (short)1,true},
                new object[]{ typeof(string),"1", typeof(ushort), (ushort)1, true },
                new object[]{ typeof(string),"1", typeof(uint), (uint)1, true },
                new object[]{ typeof(string),"1", typeof(ulong), (ulong)1, true },
                new object[]{ typeof(string),"1", typeof(long), 1L, true },
                new object[]{ typeof(string),"1", typeof(double), 1.0, true },
                new object[]{ typeof(string),"1.0", typeof(decimal), 1.0M, true },
                new object[]{ typeof(string),"1", typeof(string), "1", true },
                new object[]{ typeof(DateTime),new DateTime(2024,7,8),typeof(DateTime), new DateTime(2024,7,8),true},
                new object[]{ typeof(DateTimeOffset),new DateTimeOffset(new DateTime(2024, 7, 8)), typeof(DateTimeOffset), new DateTimeOffset(new DateTime(2024, 7, 8)), true},
                new object[]{ typeof(Guid),new Guid("C7BD06E4-DFFB-4110-860C-9DC36523E9A9"), typeof(Guid), new Guid("C7BD06E4-DFFB-4110-860C-9DC36523E9A9"), true},
            };
        }


    }
}
