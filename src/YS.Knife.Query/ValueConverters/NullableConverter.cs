using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace YS.Knife.Query.ValueConverters
{
    internal class NullableConverter : IValueConverter
    {
        public bool CanConvertTo(Type fromType, Type toType)
        {
            return fromType.IsValueType && Nullable.GetUnderlyingType(toType) == fromType;

        }

        public object Convert(object fromValue, Type toType)
        {
            return fromValue;
        }

    }
}
