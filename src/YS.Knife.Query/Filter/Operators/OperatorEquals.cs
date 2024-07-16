using System;
using System.ComponentModel;
using System.Linq.Expressions;
using YS.Knife.Query.ExpressionConverters;
using YS.Knife.Query.Expressions;
using YS.Knife.Query.ValueConverters;

namespace YS.Knife.Query.Filter.Operators
{
    internal class OperatorEquals : BaseExpressionOperator
    {
        public override Operator Operator { get; }

        public override ValueExpressionDesc CreatePredicateExpression(OperatorExpressionContext context)
        {
            var leftNode = context.Left;
            var rightNode = context.Right;
            if (leftNode.ValueType == rightNode.ValueType)
            {
                return new ValueExpressionDesc
                {
                    ValueType = typeof(bool),
                    Expression = DoOperatorAction(leftNode, rightNode)
                };
            }

            if (leftNode.IsConstant && rightNode.IsConstant)
            {
                if (ValueConverterFactory.CanConverter(rightNode.ValueType, leftNode.ValueType, out var converter1))
                {
                    return new ValueExpressionDesc
                    {
                        ValueType = typeof(bool),
                        Expression = DoOperatorAction(
                            leftNode,
                            RebuildConstantValue(rightNode, leftNode.ValueType, converter1))
                    };
                }
                else if (ValueConverterFactory.CanConverter(leftNode.ValueType, rightNode.ValueType, out var converter2))
                {
                    return new ValueExpressionDesc
                    {
                        ValueType = typeof(bool),
                        Expression = DoOperatorAction(
                           RebuildConstantValue(leftNode, rightNode.ValueType, converter2), rightNode)
                    };
                }
                else
                {
                    // 两个常量不能转换
                    throw new Exception();
                }
            }
            else if (leftNode.IsConstant && !rightNode.IsConstant)
            {
                if (ValueConverterFactory.CanConverter(leftNode.ValueType, rightNode.ValueType, out var converter3))
                {
                    return new ValueExpressionDesc
                    {
                        ValueType = typeof(bool),
                        Expression = DoOperatorAction(
                           RebuildConstantValue(leftNode, rightNode.ValueType, converter3), rightNode)
                    };
                }
                else
                {
                    // 常量不能转为变量的类型
                    throw new Exception();
                }
            }
            else if (!leftNode.IsConstant && rightNode.IsConstant)
            {
                if (ValueConverterFactory.CanConverter(rightNode.ValueType, leftNode.ValueType, out var converter4))
                {
                    return new ValueExpressionDesc
                    {
                        ValueType = typeof(bool),
                        Expression = DoOperatorAction(
                            leftNode,
                            RebuildConstantValue(rightNode, leftNode.ValueType, converter4))
                    };
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
                    return new ValueExpressionDesc
                    {
                        ValueType = typeof(bool),
                        Expression = DoOperatorAction(
                            leftNode,
                            RebuildExpressionValue(rightNode, leftNode.ValueType, converter5))
                    };
                }
                else if (ExpressionConverterFactory.CanConverter(leftNode.ValueType, rightNode.ValueType, out var converter6))
                {
                    return new ValueExpressionDesc
                    {
                        ValueType = typeof(bool),
                        Expression = DoOperatorAction(
                            RebuildExpressionValue(leftNode,rightNode.ValueType,converter6),
                            rightNode)
                    };
                }
                else
                {
                    //表达式不能相互转换
                    throw new Exception();
                }
            }
        }
       

        protected ValueExpressionDesc RebuildConstantValue(ValueExpressionDesc from, Type targetType, IValueConverter converter)
        {
            var originalExpression = (ConstantExpression)from.Expression;
            var targetValue = converter.Convert(originalExpression.Value, targetType);
            return new ValueExpressionDesc
            {
                IsConstant = true,
                ValueType = targetType,
                Expression = Expression.Constant(targetValue, targetType)
            };
        }
        protected ValueExpressionDesc RebuildExpressionValue(ValueExpressionDesc from, Type targetType, IExpressionConverter converter)
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

        protected virtual Expression DoOperatorAction(ValueExpressionDesc left, ValueExpressionDesc right)
        {
            return Expression.Equal(left.Expression, right.Expression);
        }
    }


}
