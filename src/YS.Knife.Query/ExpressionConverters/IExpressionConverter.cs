using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace YS.Knife.Query.ExpressionConverters
{
    internal interface IExpressionConverter
    {
        bool CanConvertTo(Type fromType, Type toType);
        Expression Convert(Expression expression, Type toType);
    }
}
