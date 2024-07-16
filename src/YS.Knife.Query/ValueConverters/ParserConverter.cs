using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace YS.Knife.Query.ValueConverters
{
    internal class ParserConverter : IValueConverter
    {
        public virtual bool CanConvertTo(Type fromType, Type toType)
        {
            return fromType == typeof(string) && ParseMethodFinder.GetParseMethod(toType) != null;
        }

        public virtual object Convert(object fromValue, Type toType)
        {
            var method = ParseMethodFinder.GetParseMethod(toType);
            return method.Invoke(null, new object[] { fromValue });
        }
        internal class ParseMethodFinder
        {
            static ConcurrentDictionary<Type, MethodInfo>
                methods = new ConcurrentDictionary<Type, MethodInfo>();
            public static MethodInfo GetParseMethod(Type type)
            {
                return methods.GetOrAdd(type, (t) =>
                    t.GetMethod(nameof(int.Parse), BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(string) }, null)
                );

            }
        }
    }
}
