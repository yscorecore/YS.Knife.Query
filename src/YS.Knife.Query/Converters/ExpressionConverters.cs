﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YS.Knife.Query.Converters
{
    internal class ExpressionConverters
    {
        static List<IExpressionConverter> allConverters = new List<IExpressionConverter>
        {
            new NullableConverter(),
            new SubTypeConverter(),
            new ImplicitNumberConverter(),
            new NullableImplicitNumberConverter(),
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
    }
}
