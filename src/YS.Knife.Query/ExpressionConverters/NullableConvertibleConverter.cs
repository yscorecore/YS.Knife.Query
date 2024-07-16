using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace YS.Knife.Query.ExpressionConverters
{
    internal class NullableConvertibleConverter : ConvertibleConverter
    {
        public override bool CanConvertTo(Type fromType, Type toType)
        {
            var toType2 = Nullable.GetUnderlyingType(toType);
            var fromType2 = Nullable.GetUnderlyingType(fromType);
            return toType2 != null && base.CanConvertTo(fromType2 ?? fromType, toType2);
        }
        public override Expression Convert(Expression expression, Type toType)
        {
            var toType2 = Nullable.GetUnderlyingType(toType);
            var baseExp = base.Convert(expression, toType2);
            return Expression.Convert(baseExp, toType);
        }
    }
}
