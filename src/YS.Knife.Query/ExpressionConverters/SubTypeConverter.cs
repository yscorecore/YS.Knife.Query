using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace YS.Knife.Query.ExpressionConverters
{
    internal class SubTypeConverter : IExpressionConverter
    {
        public bool CanConvertTo(Type fromType, Type toType)
        {
            return toType.IsAssignableFrom(fromType);
        }

        public Expression Convert(Expression expression, Type toType)
        {
            return Expression.Convert(expression, toType);
        }
    }
}
