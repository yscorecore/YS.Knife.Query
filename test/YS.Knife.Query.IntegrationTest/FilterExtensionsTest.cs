using System;
using System.Collections;
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
        [Fact]
        public void Debug()
        {
            ConstantEqualsTest(1, "1.0", true);
        }

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
            var filter2 = new FilterInfo
            {
                OpType = CombinSymbol.SingleItem,
                Left = rightValue,
                Right = leftValue,
                Operator = Operator.Equals
            };
            var source = new int[] { 1 };
            var target = source.AsQueryable().DoFilter(filter).ToArray();
            target.Should().BeEquivalentTo(source.Where(p => equals).ToArray());

            var target2 = source.AsQueryable().DoFilter(filter2).ToArray();
            target2.Should().BeEquivalentTo(source.Where(p => equals).ToArray());

        }

        public static IEnumerable<object[]> GetConstantEqualsData()
        {
            return new List<object[]>
            {
                new object[]{ 1, 1, true},
                new object[]{ 1, (short)1,true},
                new object[]{ 1, (ushort)1, true },
                new object[]{ 1, (uint)1, true },
                new object[]{ 1, (ulong)1, true },
                new object[]{ 1, 1L, true },
                new object[]{ 1, 1.0, true },
                new object[]{ 1, 1.0M, true },
                new object[]{ 1, "1", true },
                new object[]{ 1, "1.0", true },
                new object[]{ 1L, 1, true},
                new object[]{ 1L, (short)1,true},
                new object[]{ 1L, (ushort)1, true },
                new object[]{ 1L, (uint)1, true },
                new object[]{ 1L, (ulong)1, true },
                new object[]{ 1L, 1L, true },
                new object[]{ 1L, 1.0, true },
                new object[]{ 1L, 1.0M, true },
                new object[]{ 1L, "1", true },
                new object[]{ 1L, "1.0", true },
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
                new object[]{ "1", 1.0M, true },
                new object[]{ "1", "1", true },
                new object[]{ "1", "1.0", true },
                new object[]{ new DateTime(2024,7,8),new DateTime(2024,7,8),true},
            };
        }
        [Theory]
        [MemberData(nameof(GetValuePathAndConstantEqualsTestData))]
        public void ValuePathAndConstantEqualsTest(Type itemType, object pathValue, object constantValue)
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
            var method = this.GetType().GetMethod(nameof(GetFiltedResult), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .MakeGenericMethod(itemType);
            IList res = (IList)(method.Invoke(this, new object[] { pathValue, filter }));
            res.Count.Should().Be(1);
        }
        List<Entity1<T>> GetFiltedResult<T>(T val, FilterInfo filterInfo)
        {
            return getTestSource<T>(val).DoFilter(filterInfo).ToList();
        }
        IQueryable<Entity1<T>> getTestSource<T>(T val)
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
                new object[]{ typeof(string),"abc","abc"},
                new object[]{ typeof(string), "1" , 1},
                new object[]{ typeof(int?), 1 , 1},
                new object[]{ typeof(int?), null , null},
                new object[]{ typeof(int), 1 , 1L},
                new object[]{ typeof(int?), 1 , 1L},
                new object[]{ typeof(int), 1 , 1.0},
                new object[]{ typeof(int?), 1 , 1.0},
                new object[]{ typeof(int), 1 , 1.0M},
                new object[]{ typeof(int?), 1 , 1.0M},
                new object[]{ typeof(int), 1 , "1"},
                new object[]{ typeof(int?), 1 , "1"},

            };
        }
        record Entity1<T>
        {
            public T Val { get; set; }
        }
    }
}
