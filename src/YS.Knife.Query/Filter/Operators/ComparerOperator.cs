using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using YS.Knife.Query.Expressions;

namespace YS.Knife.Query.Filter.Operators
{
    internal class ComparerOperator : IExpressionOperator
    {
        public ComparerOperator(Operator @operator)
        {
            this.Operator = @operator;
        }
        public Operator Operator { get; private set; }
        private static MethodInfo StaticStringCompare = typeof(string)
            .GetMethod(nameof(string.Compare),
            BindingFlags.Static | BindingFlags.Public, null,
            new Type[] { typeof(string), typeof(string) }, null);
        public ValueExpressionDesc CreatePredicateExpression(OperatorExpressionContext context)
        {
            var (left, right) = LambdaUtils.ConvertToSameType(context.Left, context.Right);
            return ValueExpressionDesc.FromExpression(DoOperatorAction(left, right));
        }
        private Func<Expression, Expression, BinaryExpression> GetExpressionMethod()
        {
            return Operator switch
            {
                Operator.GreaterThan => Expression.GreaterThan,
                Operator.GreaterThanOrEqual => Expression.GreaterThanOrEqual,
                Operator.LessThan => Expression.LessThan,
                Operator.LessThanOrEqual => Expression.LessThanOrEqual,
                _ => throw new InvalidOperationException("invalid operator type"),
            };

        }
        private Expression DoOperatorAction(ValueExpressionDesc left, ValueExpressionDesc right)
        {
            Debug.Assert(left.ValueType == right.ValueType);
            if (left.ValueType == typeof(string))
            {
                return GetExpressionMethod()(Expression.Call(null, StaticStringCompare,
                     left.Expression, right.Expression), Expression.Constant(0));
            }
            else if (left.ValueType.SupportsComparisonOperators())
            {
                return GetExpressionMethod()(left.Expression, right.Expression);
            }
            else
            {
                throw new Exception($"Operator '{Operator}' does not support type '{left.ValueType.FullName}'.");
            }

        }
    }


}
