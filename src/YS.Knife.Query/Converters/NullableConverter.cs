using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace YS.Knife.Query.Converters
{
    internal class NullableConverter : IValueConverter, IExpressionConverter
    {
        public bool CanConvertTo(Type fromType, Type toType)
        {
            return fromType.IsValueType && Nullable.GetUnderlyingType(toType) == fromType;

        }

        public object Convert(object fromValue, Type toType)
        {
            return fromValue;
        }

        public Expression Convert(Expression expression, Type toType)
        {
            return Expression.Convert(expression, toType);
        }


    }
}
