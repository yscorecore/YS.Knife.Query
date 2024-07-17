using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YS.Knife.Query.ValueConverters
{
    internal class TypeConverterConverter : IValueConverter
    {
        public bool CanConvertTo(Type fromType, Type toType)
        {
            var converter = TypeDescriptor.GetConverter(toType);
            return converter.CanConvertFrom(fromType); 
        }

        public object Convert(object fromValue, Type toType)
        {
            var converter = TypeDescriptor.GetConverter(toType);

            return converter.ConvertFrom(fromValue);
        }
    }
}
