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
        static System.Collections.Concurrent.ConcurrentDictionary<string, PropertyInfo> caches = new System.Collections.Concurrent.ConcurrentDictionary<string, PropertyInfo>();
        public static PropertyInfo GetProertyOrField(Type type, string propertyOrField)
        {
            string key = $"{type.GUID}_{propertyOrField}";
            return caches.GetOrAdd(key, (t) => {
                return type.GetProperties().Where(p => p.Name.Equals(propertyOrField, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            });
        }
    }
}
