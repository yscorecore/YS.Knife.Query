using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YS.Knife.Query.Converters
{
    internal class ValueConverters
    {
        static List<IValueConverter> allConverters = new List<IValueConverter>
        {
            new NullableConverter(),
            new SubTypeConverter(),
            new BasicConverter(),
            new NullableBasicConverter(),
        };
        public static IValueConverter GetConverter(Type fromType, Type toType)
        {
            return allConverters.Where(p => p.CanConvertTo(fromType, toType)).FirstOrDefault();
        }
    }
}
