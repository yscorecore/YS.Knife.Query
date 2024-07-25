using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YS.Knife.Query.Filter
{
    internal static class TypeUtils
    {
        private static System.Collections.Concurrent.ConcurrentDictionary<Type, bool>
            cache = new System.Collections.Concurrent.ConcurrentDictionary<Type, bool>();
        //https://learn.microsoft.com/zh-cn/dotnet/csharp/language-reference/operators/comparison-operators#less-than-operator-
        private static HashSet<Type> ComparisonTypes = new HashSet<Type>
        {
            typeof(sbyte),
            typeof(byte),
            typeof(short),
            typeof(ushort),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),
            typeof(nint),
            typeof(uint),
            typeof(float),
            typeof(double),
            typeof(decimal),
            typeof(char),
            typeof(DateTime),
            typeof(DateTimeOffset),


        };
        public static bool SupportsComparisonOperators(this Type type)
        {
            var actualType = Nullable.GetUnderlyingType(type) ?? type;
            return ComparisonTypes.Contains(actualType)|| cache.GetOrAdd(actualType,
                x => x.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                      .Where(m => m.IsSpecialName && (m.Name == "op_LessThan")).Any());
        }

        private static readonly ConcurrentDictionary<Type, Type> EnumerableLocalCache = new ConcurrentDictionary<Type, Type>();
        private static readonly ConcurrentDictionary<Type, Type> QueryableLocalCache = new ConcurrentDictionary<Type, Type>();


        public static bool IsNullableType(this Type type)
        {
            return type != null && Nullable.GetUnderlyingType(type) != null;
        }

        public static (bool IsNullbale, Type UnderlyingType) GetUnderlyingTypeTypeInfo(this Type type)
        {
            return type.IsNullableType() ? (true, Nullable.GetUnderlyingType(type)) : (false, type);
        }


        public static Type GetEnumerableItemType(this Type enumerableType)
        {
            if (enumerableType == typeof(string)) return null;
            if (enumerableType.IsGenericTypeDefinition) return null;
            return EnumerableLocalCache.GetOrAdd(enumerableType, type =>
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    return type.GetGenericArguments().First();
                }
                return type.GetInterfaces()
                      .Where(p => p.IsGenericType && p.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                      .Select(p => p.GetGenericArguments().First()).FirstOrDefault();
            }
            );
        }
        public static Type GetQueryableItemType(this Type enumerableType)
        {
            if (enumerableType.IsGenericTypeDefinition) return null;
            return QueryableLocalCache.GetOrAdd(enumerableType, type =>
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IQueryable<>))
                {
                    return type.GetGenericArguments().First();
                }
                return type.GetInterfaces()
                    .Where(p => p.IsGenericType && p.GetGenericTypeDefinition() == typeof(IQueryable<>))
                    .Select(p => p.GetGenericArguments().First()).FirstOrDefault();
            }
            );
        }

        public static bool IsGenericEnumerable(this Type type) => GetEnumerableItemType(type) != null;
        public static bool IsGenericQueryable(this Type type) => GetQueryableItemType(type) != null;

    }
}
