using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace YS.Knife.Query.ExpressionConverters
{
    internal class ConvertibleConverter : IExpressionConverter
    {
        public virtual bool CanConvertTo(Type fromType, Type toType)
        {
            return typeof(IConvertible).IsAssignableFrom(fromType)
                && typeof(IConvertible).IsAssignableFrom(toType)
                && ToMethodFinder.GetConvertMethod(fromType, toType) != null;
        }

        public virtual Expression Convert(Expression expression, Type toType)
        {
            var method = ToMethodFinder.GetConvertMethod(expression.Type, toType);
            return Expression.Call(null, method, expression);
        }

        internal class ToMethodFinder
        {
            static Dictionary<string, MethodInfo> methods = typeof(Convert)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(p => p.GetParameters().Length == 1 && p.Name.StartsWith("To"))
                .ToDictionary(p => $"{p.GetParameters()[0].ParameterType.Name}{p.Name}");


            public static MethodInfo GetConvertMethod(Type fromType, Type toType)
            {
                var key = $"{fromType.Name}To{toType.Name}";
                if (methods.TryGetValue(key, out var method))
                {
                    return method;
                }
                return null;

            }
        }

    }
}
