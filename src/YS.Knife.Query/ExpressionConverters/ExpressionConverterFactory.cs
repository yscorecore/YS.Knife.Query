using System;
using System.Collections.Generic;
using System.Linq;

namespace YS.Knife.Query.ExpressionConverters
{
    internal class ExpressionConverterFactory
    {
        static List<IExpressionConverter> allConverters = new List<IExpressionConverter>
        {
            new NullableConverter(),
            new SubTypeConverter(),
            new ImplicitNumberConverter(),
            new NullableImplicitNumberConverter(),
            new EnumConverter(),
            new NullableEnumConverter(),
            new ExplicitNumberConverter(),
            new NullableExplicitNumberConverter(),
            new ConvertibleConverter(),
            new NullableConvertibleConverter(),
            new ToStringConverter(),
        };
        public static IExpressionConverter GetConverter(Type fromType, Type toType)
        {
            int? v = 1;
            double? v2 = v;
            var v3 = v2 > v;
            return allConverters.Where(p => p.CanConvertTo(fromType, toType)).FirstOrDefault();
        }
        public static bool CanConverter(Type fromType, Type toType, out IExpressionConverter converter)
        {
            converter = GetConverter(fromType, toType);
            return converter != null;
        }
    }
}
