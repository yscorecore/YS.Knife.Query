using System;
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
        };
        public static IExpressionConverter GetConverter(Type fromType, Type toType)
        {
            return allConverters.Where(p => p.CanConvertTo(fromType, toType)).FirstOrDefault();
        }
    }
}
