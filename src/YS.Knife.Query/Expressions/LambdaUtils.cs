using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using YS.Knife.Query.ExpressionConverters;
using YS.Knife.Query.ValueConverters;

namespace YS.Knife.Query.Expressions
{
    internal partial class LambdaUtils
    {
        public static Func2LambdaExpressionDesc CreateFunc2Lambda(Type fromType, List<ValuePath> paths, bool caseNullable)
        {
            var p = Expression.Parameter(fromType, "p");
            var valueExpression = ExecuteValuePaths(p, paths);
            if (Nullable.GetUnderlyingType(valueExpression.ValueType) == null && caseNullable)
            {
                var nullableType = typeof(Nullable<>).MakeGenericType(valueExpression.ValueType);
                valueExpression = new ValueExpressionDesc
                {
                    ValueType = nullableType,
                    Expression = Expression.Convert(valueExpression.Expression, nullableType)
                };

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

        public static ValueExpressionDesc ExecuteValueInfo(ParameterExpression p, ValueInfo value)
        {
            if (value.IsConstant)
            {
                return ExecuteConstantValue(p, value);
            }
            else
            {
                return ExecuteValuePaths(p, value.NavigatePaths);
            }
        }
        private static ValueExpressionDesc ExecuteValuePaths(ParameterExpression p, List<ValuePath> valuePaths)
        {
            return ExecuteValuePaths(new ValueNavigateContext(p), valuePaths);
        }
        private static ValueExpressionDesc ExecuteValuePaths(ValueNavigateContext context, List<ValuePath> valuePaths)
        {
            foreach (var vp in valuePaths)
            {
                context.LastExpression = ExecuteValuePath(context, vp);
            }
            return context.LastExpression;
        }
        private static ValueExpressionDesc ExecuteConstantValue(ParameterExpression p, ValueInfo value)
        {
            Debug.Assert(value.IsConstant);
            var exp = Expression.Constant(value.ConstantValue);
            return new ValueExpressionDesc
            {
                Expression = exp,
                ValueType = exp.Type,
                IsConstant = true,
                IsNull = value.ConstantValue == null
            };
        }

        private static ValueExpressionDesc ExecuteValuePath(ValueNavigateContext context, ValuePath valuePath)
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
        private static ValueExpressionDesc ExecuteMemberPath(ValueNavigateContext context, ValuePath valuePath)
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
                return new ValueExpressionDesc
                {
                    Expression = expression,
                    ValueType = property.PropertyType
                };
            }
            else
            {
                do
                {
                    var property = PropertyFinder.GetProertyOrField(context.LastParameter.Type, valuePath.Name);
                    if (property != null)
                    {
                        var expression = Expression.Property(context.LastParameter, property);
                        return new ValueExpressionDesc
                        {
                            Expression = expression,
                            ValueType = property.PropertyType
                        };
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
        private static ValueExpressionDesc ExecuteFunctionPath(ValueNavigateContext context, ValuePath valuePath)
        {
            throw new NotSupportedException();
        }

        private static ValueExpressionDesc RebuildConstantValue(ValueExpressionDesc from, Type targetType, IValueConverter converter)
        {
            var originalExpression = (ConstantExpression)from.Expression;
            var targetValue = converter.Convert(originalExpression.Value, targetType);
            return new ValueExpressionDesc
            {
                IsNull = targetValue == null,
                IsConstant = true,
                ValueType = targetType,
                Expression = Expression.Constant(targetValue, targetType)
            };
        }
        private static ValueExpressionDesc RebuildNullConstantValue(Type targetType)
        {
            return new ValueExpressionDesc
            {
                IsNull = true,
                IsConstant = true,
                ValueType = targetType,
                Expression = Expression.Constant(null, targetType)
            };
        }
        private static ValueExpressionDesc RebuildNullableExpressionValue(ValueExpressionDesc from,Type nullableType)
        {
            return new ValueExpressionDesc
            {
                IsNull = false,
                IsConstant = false,
                ValueType = nullableType,
                Expression = Expression.Convert(from.Expression, nullableType)
            };
        }
        private static ValueExpressionDesc RebuildExpressionValue(ValueExpressionDesc from, Type targetType, IExpressionConverter converter)
        {
            var originalExpression = from.Expression;
            var targetExpression = converter.Convert(originalExpression, targetType);
            return new ValueExpressionDesc
            {
                IsConstant = false,
                ValueType = targetType,
                Expression = targetExpression
            };
        }
        private static bool IsValueType(Type type)
        {
            return type.IsValueType && Nullable.GetUnderlyingType(type) == null;
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
                            RebuildNullableExpressionValue(rightNode,nullableType));
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
    }
}
