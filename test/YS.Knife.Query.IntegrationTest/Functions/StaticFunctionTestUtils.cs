using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;

namespace YS.Knife.Query.IntegrationTest.Functions
{
    public class StaticFunctionTestUtils
    {
        public static void CompareConstantAndFunction(Operator @operator, Type constValueType, object constValue, string functionName, ValueInfo[] arguments, bool result)
        {
            AssertObjectType(constValueType, constValue);
            var leftValue = ValueInfo.FromConstantValue(constValue);
            var rightValue = new ValueInfo
            {
                NavigatePaths = new List<ValuePath>
                {
                    new ValuePath{ Name=functionName,IsFunction=true,FunctionArgs=arguments},
                }
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

        public static void CompareFunctionAndConstant(Operator @operator, Type constValueType, object constValue, string functionName, ValueInfo[] arguments, bool result)
        {
            AssertObjectType(constValueType, constValue);

            var leftValue = new ValueInfo
            {
                NavigatePaths = new List<ValuePath>
                {
                    new ValuePath{ Name=functionName,IsFunction=true,FunctionArgs=arguments},
                }
            };
            var rightValue = ValueInfo.FromConstantValue(constValue);
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


        public static void ComparePathAndFunction(Operator @operator, Type constValueType, object constValue, string functionName, ValueInfo[] arguments, bool result)
        {
            AssertObjectType(constValueType, constValue);
            var leftValue = new ValueInfo
            {
                NavigatePaths = new List<ValuePath>
                {
                  new ValuePath { Name=nameof(Entity1<object>.Val) }
                }
            };
            var rightValue = new ValueInfo
            {
                NavigatePaths = new List<ValuePath>
                {
                    new ValuePath{ Name=functionName,IsFunction=true,FunctionArgs=arguments},
                }
            };
            var filter = new FilterInfo
            {
                OpType = CombinSymbol.SingleItem,
                Left = leftValue,
                Right = rightValue,
                Operator = @operator
            };
            var genericMethod = typeof(StaticFunctionTestUtils).GetMethod(nameof(GetEntity1DataCount), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            var method = genericMethod.MakeGenericMethod(constValueType);
            var res = method.Invoke(null, new object[] { constValue, filter });
            res.Should().Be(Convert.ToInt32(result));
        }

        public static void CompareFunctionAndPath(Operator @operator, Type constValueType, object constValue, string functionName, ValueInfo[] arguments, bool result)
        {
            AssertObjectType(constValueType, constValue);

            var leftValue = new ValueInfo
            {
                NavigatePaths = new List<ValuePath>
                {
                    new ValuePath{ Name=functionName,IsFunction=true,FunctionArgs=arguments},
                }
            };
            var rightValue = new ValueInfo
            {
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
            var genericMethod = typeof(StaticFunctionTestUtils).GetMethod(nameof(GetEntity1DataCount), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            var method = genericMethod.MakeGenericMethod(constValueType);
            var res = method.Invoke(null, new object[] { constValue, filter });
            res.Should().Be(Convert.ToInt32(result));
        }


        public static void CompareFunctionAndFunction(Operator @operator, string functionName, ValueInfo[] leftArguments, ValueInfo[] rightArguments, bool result)
        {
            var leftValue = new ValueInfo
            {
                NavigatePaths = new List<ValuePath>
                {
                    new ValuePath{ Name=functionName,IsFunction=true,FunctionArgs=leftArguments},
                }
            };
            var rightValue = new ValueInfo
            {
                NavigatePaths = new List<ValuePath>
                {
                    new ValuePath{ Name=functionName,IsFunction=true,FunctionArgs=rightArguments},
                }
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



        record Entity1<T1>
        {
            public T1 Val { get; set; }
        }

    }
}
