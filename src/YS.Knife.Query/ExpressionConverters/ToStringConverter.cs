using System;
using System.Linq.Expressions;
using System.Reflection;

namespace YS.Knife.Query.ExpressionConverters
{
    internal class ToStringConverter : IExpressionConverter
    {
        static MethodInfo ToStringMethod = typeof(object).GetMethod(nameof(object.ToString), BindingFlags.Instance | BindingFlags.Public, null, Type.EmptyTypes, null);
        public bool CanConvertTo(Type fromType, Type toType)
        {
            return toType == typeof(string);
        }
        public Expression Convert(Expression expression, Type toType)
        {
            return Expression.Call(expression, ToStringMethod);
        }
    }
}
