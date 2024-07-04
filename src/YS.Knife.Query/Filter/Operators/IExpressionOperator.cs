using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace YS.Knife.Query.Filter.Operators
{
    internal interface IExpressionOperator
    {
        Operator Operator { get; }
        Expression CreatePredicateExpression(ExpressionOperatorContext context);
    }
}
