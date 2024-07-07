using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YS.Knife.Query.Expressions;

namespace YS.Knife.Query.Filter.Operators
{
    internal class OperatorEquals : BaseExpressionOperator
    {
        public override Operator Operator { get; }

        public override ValueExpressionDesc CreatePredicateExpression(OperatorExpressionContext context)
        {
            return new ValueExpressionDesc
            {
                ValueType = typeof(bool),
                Expression = System.Linq.Expressions.Expression.Equal(context.Left.Expression, context.Right.Expression)
            };
        }
    }
}
