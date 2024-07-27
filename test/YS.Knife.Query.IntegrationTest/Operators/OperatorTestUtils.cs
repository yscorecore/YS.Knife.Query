using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;

namespace YS.Knife.Query.IntegrationTest.Operators
{
    public class OperatorTestUtils
    {


        public static void CompareConstantAndConstant(Operator @operator, Type leftType, object left, Type rightType, object right, bool result)
        {
            AssertObjectType(leftType, left);
            AssertObjectType(rightType, right);
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
                Operator = @operator
            };
            var source = new int[] { 1 };
            var target = source.AsQueryable().DoFilter(filter).ToArray();
            target.Should().BeEquivalentTo(source.Where(p => result).ToArray());
        }

        public static void ComparePathAndConstant(Operator @operator, Type leftType, object left, Type rightType, object right, bool result)
        {
            AssertObjectType(leftType, left);
            AssertObjectType(rightType, right);
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
                Operator = @operator
            };
            var genericMethod = typeof(OperatorTestUtils).GetMethod(nameof(GetEntity1DataCount), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            var method = genericMethod.MakeGenericMethod(leftType);
            var res = method.Invoke(null, new object[] { left, filter });
            res.Should().Be(Convert.ToInt32(result));
        }

        public static void CompareConstantAndPath(Operator @operator, Type leftType, object left, Type rightType, object right, bool result)
        {
            AssertObjectType(leftType, left);
            AssertObjectType(rightType, right);
            var leftValue = new ValueInfo
            {
                IsConstant = true,
                ConstantValue = left
            };
            var rightValue = new ValueInfo
            {
                IsConstant = false,
                NavigatePaths = new List<ValuePath>
                {
                  new ValuePath { Name=nameof(Entity1<object>.Val) }
                }
            };
           
            var filter = new FilterInfo
            {
                OpType = CombinSymbol.SingleItem,
                Left = leftValue,
                Right = rightValue,
                Operator = @operator
            };
            var genericMethod = typeof(OperatorTestUtils).GetMethod(nameof(GetEntity1DataCount), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            var method = genericMethod.MakeGenericMethod(rightType);
            var res = method.Invoke(null, new object[] { right, filter });
            res.Should().Be(Convert.ToInt32(result));
        }

        public static void ComparePathAndPath(Operator @operator, Type leftType, object left, Type rightType, object right, bool result)
        {
            AssertObjectType(leftType, left);
            AssertObjectType(rightType, right);
            var leftValue = new ValueInfo
            {
                IsConstant = false,
                NavigatePaths = new List<ValuePath>
                {
                  new ValuePath { Name=nameof(Entity2<object,object>.Val1) }
                }
            };
            var rightValue = new ValueInfo
            {
                IsConstant = false,
                NavigatePaths = new List<ValuePath>
                {
                  new ValuePath { Name=nameof(Entity2<object,object>.Val2) }
                }
            };

            var filter = new FilterInfo
            {
                OpType = CombinSymbol.SingleItem,
                Left = leftValue,
                Right = rightValue,
                Operator = @operator
            };
            var genericMethod = typeof(OperatorTestUtils).GetMethod(nameof(GetEntity2DataCount), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            var method = genericMethod.MakeGenericMethod(leftType, rightType);
            var res = method.Invoke(null, new object[] {  left,  right, filter });
            res.Should().Be(Convert.ToInt32(result));
        }

        private static void AssertObjectType(Type type, object value)
        {
            if (value == null)
            {
                if (type.IsValueType && Nullable.GetUnderlyingType(type) == null)
                {
                    throw new Exception($"can not assin null to type '{type}'");
                }
            }
            else
            {
                if (!type.IsAssignableFrom(value.GetType()))
                {
                    throw new Exception("type and value not match."); 
                }
            }
        }
        private static int GetEntity1DataCount<T1>(T1 t1, FilterInfo filterInfo)
        {
            var res = GetEntity1TestData(t1);
            res = res.DoFilter(filterInfo);
            return res.Count();
        }
        private static IQueryable<Entity1<T1>> GetEntity1TestData<T1>(T1 t1)
        {
            var data = new Entity1<T1>
            {
                Val = t1
            };
            return new Entity1<T1>[] { data }.AsQueryable();
        }

        private static int GetEntity2DataCount<T1,T2>(T1 t1, T2 t2, FilterInfo filterInfo)
        {
            var res = GetEntity2TestData(t1,t2);
            res = res.DoFilter(filterInfo);
            return res.Count();
        }
        private static IQueryable<Entity2<T1,T2>> GetEntity2TestData<T1,T2>(T1 t1,T2 t2)
        {
            var data = new Entity2<T1, T2>
            {
                Val1 = t1,
                Val2 = t2
            };
            return new Entity2<T1, T2>[] { data }.AsQueryable();
        }


        record Entity1<T1>
        {
            public T1 Val { get; set; }
        }
        record Entity2<T1, T2>
        {
            public T1 Val1 { get; set; }
            public T2 Val2 { get; set; }
        }

    }
}
