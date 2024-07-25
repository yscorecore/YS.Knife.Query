using System.Diagnostics;
using System.Linq.Expressions;
using YS.Knife.Query.Expressions;

namespace YS.Knife.Query.Filter.Operators
{
    internal abstract class NotResultOperator : IExpressionOperator
    {
        public NotResultOperator(bool isNot, Operator @operator)
        {
            IsNot = isNot;
            Operator = @operator;
        }

        public bool IsNot { get;  }
        public Operator Operator { get; }

        public ValueExpressionDesc CreatePredicateExpression(OperatorExpressionContext context)
        {
            Debug.Assert(context.Left.ValueType == context.Left.ValueType);
            var exp = DoOperatorAction(context.Left, context.Right);
            if (IsNot)
            {
                exp = Expression.Not(exp);
            }
            return new ValueExpressionDesc
            {
                ValueType = typeof(bool),
                Expression = exp
            };
        }
        protected abstract Expression DoOperatorAction(ValueExpressionDesc left, ValueExpressionDesc right);

    }
}
