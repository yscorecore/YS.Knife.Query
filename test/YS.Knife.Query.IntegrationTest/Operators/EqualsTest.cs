using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace YS.Knife.Query.IntegrationTest.Operators
{
    public class EqualsTest
    {

        #region ConstantCompareConstant
        [Theory]
        [MemberData(nameof(GetConstantEqualsData))]
        public void ConstantEqualsTest(object left, object right, bool equals)
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
            target.Should().BeEquivalentTo(source.Where(p => equals).ToArray());


        }

        public static IEnumerable<object[]> GetConstantEqualsData()
        {
            return new List<object[]>
            {
                new object[]{ 1, 1, true},
                new object[]{ 1, 2, false},
                new object[]{ 1, (short)1,true},
                new object[]{ 1, (ushort)1, true },
                new object[]{ 1, (uint)1, true },
                new object[]{ 1, (ulong)1, true },
                new object[]{ 1, 1L, true },
                new object[]{ 1, 1.0, true },
                new object[]{ 1, 1.0M, true },
                new object[]{ 1, "1", true },
                new object[]{ 1L, 1, true},
                new object[]{ 1L, (short)1,true},
                new object[]{ 1L, (ushort)1, true },
                new object[]{ 1L, (uint)1, true },
                new object[]{ 1L, (ulong)1, true },
                new object[]{ 1L, 1L, true },
                new object[]{ 1L, 1.0, true },
                new object[]{ 1L, 1.0M, true },
                new object[]{ 1L, "1", true },
                new object[]{ 1.0, 1, true},
                new object[]{ 1.0, (short)1,true},
                new object[]{ 1.0, (ushort)1, true },
                new object[]{ 1.0, (uint)1, true },
                new object[]{ 1.0, (ulong)1, true },
                new object[]{ 1.0, 1L, true },
                new object[]{ 1.0, 1.0, true },
                new object[]{ 1.0, 1.0M, true },
                new object[]{ 1.0, "1", true },
                new object[]{ 1.0, "1.0", true },
                new object[]{ 1M, 1, true},
                new object[]{ 1M, (short)1,true},
                new object[]{ 1M, (ushort)1, true },
                new object[]{ 1M, (uint)1, true },
                new object[]{ 1M, (ulong)1, true },
                new object[]{ 1M, 1L, true },
                new object[]{ 1M, 1.0, true },
                new object[]{ 1M, 1.0M, true },
                new object[]{ 1M, "1", true },
                new object[]{ 1M, "1.0", true },
                new object[]{ "1", 1, true},
                new object[]{ "1", (short)1,true},
                new object[]{ "1", (ushort)1, true },
                new object[]{ "1", (uint)1, true },
                new object[]{ "1", (ulong)1, true },
                new object[]{ "1", 1L, true },
                new object[]{ "1", 1.0, true },
                new object[]{ "1.0", 1.0M, true },
                new object[]{ "1", "1", true },
                new object[]{ new DateTime(2024,7,8),new DateTime(2024,7,8),true},
                new object[]{ new DateTime(2024,7,8),"2024-07-08",true},
                new object[]{ new DateTime(2024,7,8),"2024-07-08 00:00:00",true},
                new object[]{ new DateTimeOffset(new DateTime(2024, 7, 8)),"2024-07-08",true},
                new object[]{ new DateTimeOffset(new DateTime(2024, 7, 8)),"2024-07-08 00:00:00",true},
                new object[]{ new Guid("C7BD06E4-DFFB-4110-860C-9DC36523E9A9"), new Guid("C7BD06E4-DFFB-4110-860C-9DC36523E9A9"), true},
                new object[]{ new Guid("C7BD06E4-DFFB-4110-860C-9DC36523E9A9"), "C7BD06E4-DFFB-4110-860C-9DC36523E9A9", true},
                new object[]{ "1", null, false },
                new object[]{ 1, null, false },
            };
        }

        #endregion

        #region ConstantComparePathValue
        [Theory]
        [MemberData(nameof(GetValuePathAndConstantEqualsTestData))]
        public void ValuePathAndConstantEqualsTest(Type itemType, object pathValue, object constantValue, bool isEquals)
        {
            var leftValue = new ValueInfo
            {
                NavigatePaths = new List<ValuePath>
                {
                    new ValuePath{ Name=nameof(Entity1<string>.Val) }
                }
            };
            var rightValue = new ValueInfo
            {

                IsConstant = true,
                ConstantValue = constantValue
            };
            var filter = new FilterInfo
            {
                OpType = CombinSymbol.SingleItem,
                Left = leftValue,
                Right = rightValue,
                Operator = Operator.Equals
            };
            var method = GetType().GetMethod(nameof(GetFiltedResult), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .MakeGenericMethod(itemType);
            var res = (IList)method.Invoke(this, new object[] { pathValue, filter });
            res.Count.Should().Be(Convert.ToInt32(isEquals));
            //swap left and right
            var filter2 = new FilterInfo
            {
                OpType = CombinSymbol.SingleItem,
                Left = rightValue,
                Right = leftValue,
                Operator = Operator.Equals
            };
            var res2 = (IList)method.Invoke(this, new object[] { pathValue, filter2 });
            res2.Count.Should().Be(Convert.ToInt32(isEquals));


        }
        List<Entity1<T>> GetFiltedResult<T>(T val, FilterInfo filterInfo)
        {
            return GetTestSource(val).DoFilter(filterInfo).ToList();
        }
        IQueryable<Entity1<T>> GetTestSource<T>(T val)
        {
            return new List<Entity1<T>>
            {
                new Entity1<T>{ Val = val }
            }.AsQueryable();
        }
        public static IEnumerable<object[]> GetValuePathAndConstantEqualsTestData()
        {
            return new List<object[]>
            {
                new object[]{ typeof(string),"abc","abc",true},
                new object[]{ typeof(string),"abc","abcd",false},
                new object[]{ typeof(string), "1" , 1,true},
                new object[]{ typeof(string), "1" , 2, false },
                new object[]{ typeof(string), "1" , 1L,true},
                new object[]{ typeof(string), "1" , 1.0,true},
                new object[]{ typeof(string), "1.0" , 1.0M,true},
                new object[]{ typeof(string), "1" , null, false},
                new object[]{ typeof(int), 1 , 1, true},
                new object[]{ typeof(int), 1 , 2, false},
                new object[]{ typeof(int), 1 , 1L, true},
                new object[]{ typeof(int), 1 , 1.0, true},
                new object[]{ typeof(int), 1 , 1.0M, true},
                new object[]{ typeof(int), 1 , null, false},
                new object[]{ typeof(int?), 1 , 1, true},
                new object[]{ typeof(int?), 1 , 2, false},
                new object[]{ typeof(int?), 1 , 1L, true},
                new object[]{ typeof(int?), 1 , 1.0, true},
                new object[]{ typeof(int?), 1 , 1.0M, true},
                new object[]{ typeof(int?), 1 , null, false},
                new object[]{ typeof(int?), null , 1, false},
                new object[]{ typeof(int?), null , null, true},
            };
        }

        #endregion
        [Theory]
        [MemberData(nameof(GetValuePathAndValuePathEqualsTestData))]
        public void ValuePathAndValuePathEqualsTest(Type leftType, object leftValue, Type rightType, object rightValue, bool isEquals)
        {
            var leftValueInfo = new ValueInfo
            {
                NavigatePaths = new List<ValuePath>
                {
                    new ValuePath{ Name=nameof(Entity2<string,string>.Val1) }
                }
            };
            var rightValueInfo = new ValueInfo
            {
                NavigatePaths = new List<ValuePath>
                {
                    new ValuePath{ Name=nameof(Entity2<string,string>.Val2) }
                }
            };
            var filter = new FilterInfo
            {
                OpType = CombinSymbol.SingleItem,
                Left = leftValueInfo,
                Right = rightValueInfo,
                Operator = Operator.Equals
            };
            var method = GetType().GetMethod(nameof(GetFiltedResult2), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .MakeGenericMethod(leftType, rightType);
            var res = (IList)method.Invoke(this, new object[] { leftValue, rightValue, filter });
            res.Count.Should().Be(Convert.ToInt32(isEquals));
            //swap left and right
            var filter2 = new FilterInfo
            {
                OpType = CombinSymbol.SingleItem,
                Left = rightValueInfo,
                Right = leftValueInfo,
                Operator = Operator.Equals
            };
            var method2 = GetType().GetMethod(nameof(GetFiltedResult2), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .MakeGenericMethod(rightType, leftType);
            var res2 = (IList)method2.Invoke(this, new object[] { rightValue, leftValue, filter2 });
            res2.Count.Should().Be(Convert.ToInt32(isEquals));

        }
        public static IEnumerable<object[]> GetValuePathAndValuePathEqualsTestData()
        {
            yield return new object[] { typeof(string), "1", typeof(int), 1, true };
            yield return new object[] { typeof(string), "1", typeof(long), 1L, true };
            yield return new object[] { typeof(string), "1", typeof(double), 1.0, true };
            yield return new object[] { typeof(string), "1.0", typeof(decimal), 1.0M, true };
            yield return new object[] { typeof(string), "1", typeof(string), "1", true };

            yield return new object[] { typeof(string), "1", typeof(int?), 1, true };
            yield return new object[] { typeof(string), "1", typeof(long?), 1L, true };
            yield return new object[] { typeof(string), "1", typeof(double?), 1.0, true };
            yield return new object[] { typeof(string), "1.0", typeof(decimal?), 1.0M, true };

            yield return new object[] { typeof(int), 1, typeof(int), 1, true };
            yield return new object[] { typeof(int), 1, typeof(long), 1L, true };
            yield return new object[] { typeof(int), 1, typeof(double), 1.0, true };
            yield return new object[] { typeof(int), 1, typeof(decimal), 1.0M, true };
            yield return new object[] { typeof(int), 1, typeof(string), "1", true };

            yield return new object[] { typeof(int), 1, typeof(int?), 1, true };
            yield return new object[] { typeof(int), 1, typeof(long?), 1L, true };
            yield return new object[] { typeof(int), 1, typeof(double?), 1.0, true };
            yield return new object[] { typeof(int), 1, typeof(decimal?), 1.0M, true };

            yield return new object[] { typeof(int?), 1, typeof(int?), 1, true };
            yield return new object[] { typeof(int?), 1, typeof(long?), 1L, true };
            yield return new object[] { typeof(int?), 1, typeof(double?), 1.0, true };
            yield return new object[] { typeof(int?), 1, typeof(decimal?), 1.0M, true };

        }
        List<Entity2<T1, T2>> GetFiltedResult2<T1, T2>(T1 val1, T2 val2, FilterInfo filterInfo)
        {
            return GetTestSource2(val1, val2).DoFilter(filterInfo).ToList();
        }
        IQueryable<Entity2<T1, T2>> GetTestSource2<T1, T2>(T1 val1, T2 val2)
        {
            return new List<Entity2<T1, T2>>
            {
                new Entity2<T1,T2>{  Val1=val1,Val2=val2 }
            }.AsQueryable();
        }
        record Entity1<T>
        {
            public T Val { get; set; }
        }

        record Entity2<T1, T2>
        {
            public T1 Val1 { get; set; }
            public T2 Val2 { get; set; }
        }
    }
}
