using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace YS.Knife.Query.Converters
{
    internal class ToStringConverter : IValueConverter, IExpressionConverter
    {
        static MethodInfo ToStringMethod = typeof(object).GetMethod(nameof(object.ToString), BindingFlags.Instance | BindingFlags.Public, null, Type.EmptyTypes, null);
        public bool CanConvertTo(Type fromType, Type toType)
        {
            return toType == typeof(string);
        }

        public object Convert(object fromValue, Type toType)
        {
            return fromValue?.ToString();
        }

        public Expression Convert(Expression expression, Type toType)
        {
            return Expression.Call(expression, ToStringMethod);
        }
    }
}
