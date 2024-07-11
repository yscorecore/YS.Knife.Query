using System;
using System.ComponentModel;
using System.Linq.Expressions;
using YS.Knife.Query.Expressions;

namespace YS.Knife.Query.Filter.Operators
{
    internal class OperatorEquals : BaseExpressionOperator
    {
        public override Operator Operator { get; }

        public override ValueExpressionDesc CreatePredicateExpression(OperatorExpressionContext context)
        {
            var leftNode = context.Left;
            var rightNode = context.Right;
            

            if (leftNode.ValueType != rightNode.ValueType)
            {
                if (leftNode.IsConstant && rightNode.IsConstant)
                {

                }
                else if (leftNode.IsConstant && !rightNode.IsConstant)
                {
                    if (CanConvertValue(leftNode.ValueType, rightNode.ValueType, out var converter))
                    {

                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                else if (!leftNode.IsConstant && rightNode.IsConstant)
                {

                }
                else
                { 
                    
                }

                if (rightNode.IsConstant)
                {
                    if (CanConvertValue(rightNode.ValueType, leftNode.ValueType, out var converter))
                    {
                        rightNode = RebuildConstantValue(rightNode, leftNode.ValueType, converter);
                    }
                }
                else if (leftNode.IsConstant)
                {
                    if (CanConvertValue(leftNode.ValueType, rightNode.ValueType, out var converter))
                    {
                        leftNode = RebuildConstantValue(leftNode, rightNode.ValueType, converter);
                    }
                }
                else
                {
                    if (CanConvertExpression(rightNode.ValueType, leftNode.ValueType))
                    {
                        rightNode = ConvertExpression(rightNode, leftNode.ValueType);
                    }
                    else if (CanConvertExpression(leftNode.ValueType, rightNode.ValueType))
                    {
                        leftNode = ConvertExpression(leftNode, rightNode.ValueType);
                    }
                }
            }
            if (leftNode.ValueType != rightNode.ValueType)
            {
                throw new QueryExpressionBuildException($"can not convert type between '{leftNode.ValueType.FullName}' and '{rightNode.ValueType.FullName}'");
            }

            return new ValueExpressionDesc
            {
                ValueType = typeof(bool),
                Expression = DoOperatorAction(leftNode.Expression, rightNode.Expression)
            };


        }

        protected ValueExpressionDesc RebuildConstantValue(ValueExpressionDesc from, Type targetType, TypeConverter converter)
        {
            var originalExpression = (ConstantExpression)from.Expression;
            var targetValue = converter.ConvertTo(originalExpression.Value, targetType);
            return new ValueExpressionDesc
            {
                IsConstant = true,
                ValueType = targetType,
                Expression = Expression.Constant(targetValue, targetType)
            };
        }
        protected ValueExpressionDesc ConvertExpression(ValueExpressionDesc from, Type targetType)
        {
            var originalExpression = (ConstantExpression)from.Expression;
            return new ValueExpressionDesc
            {
                IsConstant = true,
                ValueType = targetType,
                Expression = Expression.Constant(originalExpression.Value, targetType)
            };
        }
        protected virtual Expression DoOperatorAction(Expression left, Expression right)
        {
            return Expression.Equal(left, right);
        }

        bool CanConvertValue(Type fromType, Type toType, out TypeConverter converter)
        {
            converter = System.ComponentModel.TypeDescriptor.GetConverter(fromType);
            if (converter != null && converter.CanConvertTo(toType))
            {
                return true;
            }
            return false;
        }
        bool CanConvertExpression(Type fromType, Type toType)
        {
            if (Nullable.GetUnderlyingType(fromType) == toType)
            { 
                
            }
            return false;
        }
        enum ConvertKind
        { 
            Nullable,
            ToString,
            Convert
        }
    }
}
