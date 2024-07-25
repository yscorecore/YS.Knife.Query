using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace YS.Knife.Query.IntegrationTest.Operators
{
    public class BetweenTest
    {
        [Theory]
        [MemberData(nameof(GetTestData))]
        public void ShouldFilter_ConstantAndConstant(Type leftType, object left, Type rightType, object right, bool result)
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
                Operator = Operator.Between
            };
            var source = new int[] { 1 };
            var target = source.AsQueryable().DoFilter(filter).ToArray();
            target.Should().BeEquivalentTo(source.Where(p => result).ToArray());
        }


        [Theory]
        [MemberData(nameof(GetTestData))]
        public void ShouldFilter_PathAndConstant(Type leftType, object left, Type rightType, object right, bool result)
        {
            var leftValue = new ValueInfo
            {
                IsConstant = false,
                NavigatePaths = new List<ValuePath>
               {
                 new ValuePath { Name=nameof(Entity1<object>.Val) }
               }
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
                Operator = Operator.Between
            };
            var genericMethod = this.GetType().GetMethod(nameof(GetDataCount), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var method = genericMethod.MakeGenericMethod(leftType);
            var res = method.Invoke(this, new object[] { left, filter });
            res.Should().Be(Convert.ToInt32(result));
        }


      




        private int GetDataCount<T1>(T1 t1, FilterInfo filterInfo)
        {
            var res = GetEntity1TestData(t1);
            res = res.DoFilter(filterInfo);
            return res.Count();
        }
        private IQueryable<Entity1<T1>> GetEntity1TestData<T1>(T1 t1)
        {
            var data = new Entity1<T1>
            {
                Val = t1
            };
            return new Entity1<T1>[] { data }.AsQueryable();
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
            yield return new(typeof(int), 1, typeof(int[]), new object[] { 1, 1 }, true);
            yield return new(typeof(int), 1, typeof(int[]), new int[] { 1, 1 }, true);
            yield return new(typeof(int), 1, typeof(int[]), new int[] { 2, 3 }, false);
            yield return new(typeof(int), 1, typeof(int[]), new int[] { -1, 0 }, false);
            yield return new(typeof(int), 1, typeof(int[]), new int[] { 0, 2 }, true);
            yield return new(typeof(int), 1, typeof(int[]), new int[] { 2, 0 }, false);
            yield return new(typeof(string), "1", typeof(object[]), new object[] { "0", "2" }, true);
            yield return new(typeof(DateTime), new DateTime(2024, 7, 25), typeof(object[]), new object[] { new DateTime(2024, 7, 24), new DateTime(2024, 7, 26) }, true);
            yield return new(typeof(DateTime), new DateTime(2024, 7, 25), typeof(DateTime[]), new DateTime[] { new DateTime(2024, 7, 24), new DateTime(2024, 7, 26) }, true);

        }

        record Entity1<T1>
        {
            public T1 Val { get; set; }
        }
    }
}
