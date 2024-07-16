using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace YS.Knife.Query.ValueConverters
{
    internal class SubTypeConverter : IValueConverter
    {
        public bool CanConvertTo(Type fromType, Type toType)
        {
            return toType.IsAssignableFrom(fromType);
        }

        public object Convert(object fromValue, Type toType)
        {
            return fromValue;
        }

    }
}
