using System;
using System.Collections.Generic;
using System.Linq;

namespace YS.Knife.Query.ValueConverters
{
    internal class ValueConverterFactory
    {
        static List<IValueConverter> allConverters = new List<IValueConverter>
        {
            new NullableConverter(),
            new SubTypeConverter(),
            new TypeConverterConverter(),
            new ParserConverter(),
            new NullableParserConverter(),
            new EnumConverter(),
            new NullabaleEnumConverter(),
            new BasicConverter(),
            new NullableBasicConverter(),
            new ToStringConverter(),
        };
        public static IValueConverter GetConverter(Type fromType, Type toType)
        {
            return allConverters.Where(p => p.CanConvertTo(fromType, toType)).FirstOrDefault();
        }
        public static bool CanConverter(Type fromType, Type toType, out IValueConverter converter)
        {
            converter = GetConverter(fromType, toType);
            return converter != null;
        }
    }
}
