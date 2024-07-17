using System.Linq.Expressions;
using YS.Knife.Query.Expressions;

namespace YS.Knife.Query.Filter.Operators
{
    internal class OperatorEquals : BaseExpressionOperator
    {
        public override Operator Operator { get; }

        public override ValueExpressionDesc CreatePredicateExpression(OperatorExpressionContext context)
        {
            var (left, right) = LambdaUtils.ConvertToSameType(context.Left, context.Right);
            return new ValueExpressionDesc
            {
                Expression = DoOperatorAction(left.Expression, right.Expression),
                ValueType = typeof(bool)
            };
        }
        protected virtual Expression DoOperatorAction(Expression left, Expression right)
        {
            return Expression.Equal(left, right);
        }
    }


}
