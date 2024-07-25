using System;
using System.Linq.Expressions;
using YS.Knife.Query.Expressions;

namespace YS.Knife.Query.Filter.Operators
{
    internal class EqualsOperator : IExpressionOperator
    {
        public Operator Operator { get; set; }

        public ValueExpressionDesc CreatePredicateExpression(OperatorExpressionContext context)
        {
            var (left, right) = LambdaUtils.ConvertToSameType(context.Left, context.Right);
            return new ValueExpressionDesc
            {
                Expression = DoOperatorAction(left.Expression, right.Expression),
                ValueType = typeof(bool)
            };
        }
        private Expression DoOperatorAction(Expression left, Expression right)
        {
            return GetExpressionMethod()(left, right);
        }

        private Func<Expression, Expression, BinaryExpression> GetExpressionMethod()
        {
            return Operator switch
            {
                Operator.Equals => Expression.Equal,
                Operator.NotEquals => Expression.NotEqual,
                _ => throw new InvalidOperationException("invalid operator type"),
            };

        }
    }

}
