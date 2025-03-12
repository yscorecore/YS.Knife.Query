using System.Diagnostics;
using System.Linq.Expressions;
using YS.Knife.Query.Expressions;

namespace YS.Knife.Query.Filter.Operators
{
    internal abstract class NotResultOperator : IExpressionOperator
    {
        public NotResultOperator(Operator @operator)
        {
            IsNot = @operator < 0;
            Operator = @operator;
        }

        public bool IsNot { get; }
        public Operator Operator { get; }

        public ValueExpressionDesc CreatePredicateExpression(OperatorExpressionContext context)
        {
            Debug.Assert(context.Left.ValueType == context.Left.ValueType);
            var exp = DoOperatorAction(context.Left, context.Right);
            if (IsNot)
            {
                exp = Expression.Not(exp);
            }
            return ValueExpressionDesc.FromExpression(exp);
        }
        protected abstract Expression DoOperatorAction(ValueExpressionDesc left, ValueExpressionDesc right);

    }
}
