using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using YS.Knife.Query.Expressions;

namespace YS.Knife.Query.Filter.Operators
{
    internal interface IExpressionOperator
    {
        Operator Operator { get; }
        ValueExpressionDesc CreatePredicateExpression(ExpressionOperatorContext context);
    }
}
