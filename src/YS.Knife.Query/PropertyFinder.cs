using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace YS.Knife.Query
{
    internal class PropertyFinder
    {
        static System.Collections.Concurrent.ConcurrentDictionary<Type, Dictionary<string, PropertyInfo>> caches = new System.Collections.Concurrent.ConcurrentDictionary<Type, Dictionary<string, PropertyInfo>>();
        public static PropertyInfo GetProertyOrField(Type type, string propertyOrField)
        {
            var propMaps = caches.GetOrAdd(type, (t) =>
            {
                var dic = new Dictionary<string, PropertyInfo>(StringComparer.OrdinalIgnoreCase);
                foreach (var p in type.GetProperties())
                {
                    var propName = p.GetCustomAttribute<System.Text.Json.Serialization.JsonPropertyNameAttribute>();
                    dic[propName?.Name ?? p.Name] = p;
                }
                return dic;
            });
            if (propMaps.TryGetValue(propertyOrField, out var p))
            {
                return p;
            }
            else
            {
                throw new Exception($"can not find property '{propertyOrField}' in type '{type.FullName}'");
            }

        }
    }
}
