using System;
using System.Linq.Expressions;
using YS.Knife.Query.Expressions;

namespace YS.Knife.Query.Filter.Operators
{
    internal class EqualsOperator : IExpressionOperator
    {
        public EqualsOperator(Operator @operator)
        {
            this.Operator = @operator;
        }
        public Operator Operator { get; private set; }

        public ValueExpressionDesc CreatePredicateExpression(OperatorExpressionContext context)
        {
            var (left, right) = LambdaUtils.ConvertToSameType(context.Left, context.Right);
            return ValueExpressionDesc.FromExpression(DoOperatorAction(left.Expression, right.Expression));
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
