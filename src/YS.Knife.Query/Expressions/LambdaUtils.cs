using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using YS.Knife.Query.ExpressionConverters;
using YS.Knife.Query.Filter;
using YS.Knife.Query.ValueConverters;
using System.Linq;
using YS.Knife.Query.Functions;

namespace YS.Knife.Query.Expressions
{
    internal partial class LambdaUtils
    {
        public static Func2LambdaExpressionDesc CreateFunc2Lambda(Type fromType, List<ValuePath> paths, bool caseNullable)
        {
            var p = Expression.Parameter(fromType, "p");
            var context = new ValueExecuteContext(p);
            var valueExpression = ExecuteValuePaths(context, paths);
            if (Nullable.GetUnderlyingType(valueExpression.ValueType) == null && caseNullable)
            {
                var nullableType = typeof(Nullable<>).MakeGenericType(valueExpression.ValueType);
                valueExpression = ValueExpressionDesc.FromExpression(Expression.Convert(valueExpression.Expression, nullableType));
            }
            var funcType = typeof(Func<,>).MakeGenericType(fromType, valueExpression.ValueType);
            return new Func2LambdaExpressionDesc
            {
                SourceType = fromType,
                ValueType = valueExpression.ValueType,
                FuncType = funcType,
                Lambda = Expression.Lambda(funcType, valueExpression.Expression, p)
            };
        }

        public static ValueExpressionDesc ExecuteValueInfo(ValueExecuteContext context, ValueInfo value)
        {
            if (value.IsConstant)
            {
                return ExecuteConstantValue(context, value);
            }
            else
            {
                return ExecuteValuePaths(context, value.NavigatePaths);
            }
        }
        public static ValueExpressionDesc ExecuteValueInfoAndConvert(ValueExecuteContext context, ValueInfo value,Type type)
        {
            return ToType(ExecuteValueInfo(context, value), type);
        }
        private static ValueExpressionDesc ExecuteValuePaths(ValueExecuteContext context, List<ValuePath> valuePaths)
        {
            var currentContext = context;
            foreach (var vp in valuePaths)
            {
                var valueExpression = ExecuteValuePath(currentContext, vp);
                currentContext = currentContext.Go(valueExpression);
            }
            return currentContext.LastExpression;
        }

        private static ValueExpressionDesc ExecuteConstantValue(ValueExecuteContext _, ValueInfo value)
        {
            Debug.Assert(value.IsConstant);
            var exp = Expression.Constant(value.ConstantValue);
            return ValueExpressionDesc.FromValue(value.ConstantValue);
        }

        private static ValueExpressionDesc ExecuteValuePath(ValueExecuteContext context, ValuePath valuePath)
        {
            if (valuePath.IsFunction)
            {
                return ExecuteFunctionPath(context, valuePath);
            }
            else
            {
                return ExecuteMemberPath(context, valuePath);
            }
        }
        private static ValueExpressionDesc ExecuteMemberPath(ValueExecuteContext context, ValuePath valuePath)
        {
            var lastExpression = context.LastExpression;
            if (lastExpression != null)
            {
                var property = PropertyFinder.GetProertyOrField(context.LastExpression.ValueType, valuePath.Name);
                if (property == null)
                {
                    throw new QueryExpressionBuildException($"can not find member '{valuePath.Name}' from type '{lastExpression.ValueType.FullName}'.");
                }
                var expression = Expression.Property(lastExpression.Expression, property);
                return ValueExpressionDesc.FromExpression(expression);
            }
            else
            {
                do
                {
                    var property = PropertyFinder.GetProertyOrField(context.LastParameter.Type, valuePath.Name);
                    if (property != null)
                    {
                        var expression = Expression.Property(context.LastParameter, property);

                        return ValueExpressionDesc.FromExpression(expression);
                    }
                    else
                    {
                        context = context.Pop();
                        if (context.Deepth == 0)
                        {
                            throw new QueryExpressionBuildException($"can not find member '{valuePath.Name}' from parameters.");
                        }
                    }
                }
                while (true);
            }


        }
        private static ValueExpressionDesc ExecuteFunctionPath(ValueExecuteContext context, ValuePath valuePath)
        {
            var functionContext = new FunctionContext { Arguments = valuePath.FunctionArgs ?? Array.Empty<ValueInfo>(), ExecuteContext = context };
            var function = AllFunctions.Get(valuePath.Name);
            if (function == null)
            {
                throw new Exception($"function '{valuePath.Name}' not supported.");
            }
            return function.Execute(functionContext);
        }

        private static ValueExpressionDesc RebuildConstantValue(ValueExpressionDesc from, Type targetType, IValueConverter converter)
        {
            var originalExpression = (ConstantExpression)from.Expression;
            var targetValue = converter.Convert(originalExpression.Value, targetType);
            return ValueExpressionDesc.FromValue(targetValue, targetType);
        }
        private static ValueExpressionDesc RebuildNullConstantValue(Type targetType)
        {
            return ValueExpressionDesc.FromValue(null, targetType);
        }
        private static ValueExpressionDesc RebuildNullableExpressionValue(ValueExpressionDesc from, Type nullableType)
        {
            return ValueExpressionDesc.FromExpression(Expression.Convert(from.Expression, nullableType));
        }
        private static ValueExpressionDesc RebuildExpressionValue(ValueExpressionDesc from, Type targetType, IExpressionConverter converter)
        {
            var originalExpression = from.Expression;
            var targetExpression = converter.Convert(originalExpression, targetType);
            return ValueExpressionDesc.FromExpression(targetExpression);
        }
        private static bool IsValueType(Type type)
        {
            return type.IsValueType && Nullable.GetUnderlyingType(type) == null;
        }
        public static ValueExpressionDesc ToType(ValueExpressionDesc valueExpression, Type targetType)
        {
            if (valueExpression.ValueType == targetType)
            {
                return valueExpression;
            }

            if (valueExpression.IsConstant)
            {
                if (valueExpression.IsNull)
                {
                    return RebuildNullConstantValue(targetType);
                }
                else
                {
                    if (ValueConverterFactory.CanConverter(valueExpression.ValueType, targetType, out var converter))
                    {
                        return RebuildConstantValue(valueExpression, targetType, converter);
                    }
                    else
                    {
                        throw new Exception($"can not convert const value to {targetType.FullName}");
                    }
                }
            }
            else
            {
                if (ExpressionConverterFactory.CanConverter(valueExpression.ValueType, typeof(string), out var converter))
                {
                    return RebuildExpressionValue(valueExpression, typeof(string), converter);
                }
                else
                {
                    throw new Exception("can not convert expression value to string");
                }
            }
        }
        public static (ValueExpressionDesc Left, ValueExpressionDesc Right) ConvertToStringType(ValueExpressionDesc left, ValueExpressionDesc right)
        {
            return (ToType(left,typeof(string)), ToType(right,typeof(string)));
        
        }
        public static (ValueExpressionDesc Left, ValueExpressionDesc Right) ConvertToSameType(ValueExpressionDesc leftNode, ValueExpressionDesc rightNode)
        {
            if (leftNode.ValueType == rightNode.ValueType)
            {
                return (leftNode, rightNode);
            }

            if (leftNode.IsConstant && rightNode.IsConstant)
            {
                if (ValueConverterFactory.CanConverter(rightNode.ValueType, leftNode.ValueType, out var converter1))
                {
                    return (leftNode, RebuildConstantValue(rightNode, leftNode.ValueType, converter1));

                }
                else if (ValueConverterFactory.CanConverter(leftNode.ValueType, rightNode.ValueType, out var converter2))
                {
                    return (RebuildConstantValue(leftNode, rightNode.ValueType, converter2), rightNode);
                }
                else
                {
                    // 两个常量不能转换
                    throw new Exception();
                }
            }
            else if (leftNode.IsConstant && !rightNode.IsConstant)
            {
                if (leftNode.IsNull)
                {
                    if (IsValueType(rightNode.ValueType))
                    {
                        var nullableType = typeof(Nullable<>).MakeGenericType(rightNode.ValueType);
                        return (RebuildNullConstantValue(nullableType),
                            RebuildNullableExpressionValue(rightNode, nullableType));
                    }
                    else
                    {
                        return (RebuildNullConstantValue(rightNode.ValueType), rightNode);
                    }
                }
                else if (ValueConverterFactory.CanConverter(leftNode.ValueType, rightNode.ValueType, out var converter3))
                {
                    return (RebuildConstantValue(leftNode, rightNode.ValueType, converter3), rightNode);
                }
                else
                {
                    //常量不能转为变量的类型
                    throw new Exception();
                }
            }
            else if (!leftNode.IsConstant && rightNode.IsConstant)
            {
                if (rightNode.IsNull)
                {
                    if (IsValueType(leftNode.ValueType))
                    {
                        var nullableType = typeof(Nullable<>).MakeGenericType(leftNode.ValueType);
                        return (RebuildNullableExpressionValue(leftNode, nullableType),
                            RebuildNullConstantValue(nullableType));
                    }
                    else
                    {
                        return (leftNode, RebuildNullConstantValue(leftNode.ValueType));
                    }
                }
                else if (ValueConverterFactory.CanConverter(rightNode.ValueType, leftNode.ValueType, out var converter4))
                {
                    return (leftNode,
                            RebuildConstantValue(rightNode, leftNode.ValueType, converter4));

                }
                else
                {
                    // 常量不能转为变量的类型
                    throw new Exception();
                }
            }
            else
            {
                if (ExpressionConverterFactory.CanConverter(rightNode.ValueType, leftNode.ValueType, out var converter5))
                {
                    return (leftNode,
                            RebuildExpressionValue(rightNode, leftNode.ValueType, converter5));
                }
                else if (ExpressionConverterFactory.CanConverter(leftNode.ValueType, rightNode.ValueType, out var converter6))
                {
                    return (RebuildExpressionValue(leftNode, rightNode.ValueType, converter6),
                            rightNode);
                }
                else
                {
                    //表达式不能相互转换
                    throw new Exception();
                }
            }
        }
        private static TOut[] ConvertItemsValue<TIn, TOut>(IEnumerable<TIn> source, IValueConverter valueConverter)
        {
            return source.Select(p => (TOut)valueConverter.Convert(p, typeof(TOut))).ToArray();
        }

        private static ValueExpressionDesc RebuildConstantValueArray(object items, Type itemSourceType, Type itemTargetType, IValueConverter converter)
        {
            var method = typeof(LambdaUtils).GetMethod(nameof(ConvertItemsValue))
                .MakeGenericMethod(itemSourceType, itemTargetType);
            var targetItems = method.Invoke(null, new object[] { items, converter });
            return ValueExpressionDesc.FromValue(targetItems);
        }
        private static ValueExpressionDesc RebuildConstantValueArray(object items, Type itemSourceType, Type itemTargetType)
        {
            return RebuildConstantValueArray(items, itemSourceType, itemTargetType, ValueConverterFactory.GetConverter(itemSourceType, itemTargetType));
        }
        public static (ValueExpressionDesc Left, ValueExpressionDesc Right) ConvertToSameItemType(ValueExpressionDesc left, ValueExpressionDesc right)
        {
            var rightItemType = right.ValueType.GetEnumerableItemType();
            if (rightItemType == null)
            {
                throw new Exception("right value type is not a enumerable type");
            }
            if (rightItemType == left.ValueType)
            {
                return (left, right);
            }

            if (left.IsConstant)
            {
                if (right.IsConstant)
                {
                    // 右边转换为左边的数组
                    object rightValue = (right.Expression as ConstantExpression).Value;
                    if (left.IsNull)
                    {
                        if (IsValueType(rightItemType))
                        {
                            var nullableRightItemType = typeof(Nullable<>).MakeGenericType(rightItemType);
                            return (RebuildNullConstantValue(nullableRightItemType),
                                RebuildConstantValueArray(rightValue, rightItemType, nullableRightItemType));
                        }
                        else
                        {
                            return (RebuildNullConstantValue(rightItemType), right);
                        }
                    }
                    else
                    {
                        if (ValueConverterFactory.CanConverter(left.ValueType, rightItemType, out var converter))
                        {
                            return (RebuildConstantValue(left, rightItemType, converter), right);
                        }
                        else if (ValueConverterFactory.CanConverter(rightItemType, left.ValueType, out var converter2))
                        {
                            return (left, RebuildConstantValueArray(rightValue, rightItemType, left.ValueType, converter2));
                        }
                        else
                        {
                            throw new Exception("can not convert");
                        }
                    }
                }
                else
                {
                    //将左边转为右边的子类型
                    if (ValueConverterFactory.CanConverter(left.ValueType, rightItemType, out var converter3))
                    {
                        return (RebuildConstantValue(left, rightItemType, converter3), right);
                    }
                    else
                    {
                        throw new Exception("can not converter");
                    }
                }
            }
            else
            {
                if (right.IsConstant)
                {
                    //右边转为左边的数组
                    object rightValue = (right.Expression as ConstantExpression).Value;
                    if (ValueConverterFactory.CanConverter(rightItemType, left.ValueType, out var converter5))
                    {
                        return (left, RebuildConstantValueArray(rightValue, rightItemType, left.ValueType, converter5));
                    }
                    else
                    {
                        throw new Exception("can not convert");
                    }
                }
                else
                {
                    if (ExpressionConverterFactory.CanConverter(left.ValueType, rightItemType, out var converter6))
                    {
                        return (RebuildExpressionValue(left, rightItemType, converter6), right);
                    }
                    else
                    {
                        throw new Exception("can not convert");
                    }
                }

            }

        }
    }
}
