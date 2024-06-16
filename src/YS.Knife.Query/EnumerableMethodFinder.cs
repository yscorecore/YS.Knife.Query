using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace YS.Knife.Query
{
    internal class EnumerableMethodFinder
    {
        private static readonly Dictionary<Type, MethodInfo> SumMethodWith2Args = typeof(Enumerable).GetMethods()
          .Where(p => p.Name == nameof(Enumerable.Sum))
          .Where(p => p.GetParameters().Length == 2)
          .ToDictionary(p => p.GetParameters()[1].ParameterType.GenericTypeArguments[1]);
        private static readonly Dictionary<Type, MethodInfo> MinMethodWith2Args = typeof(Enumerable).GetMethods()
            .Where(p => p.Name == nameof(Enumerable.Min))
            .Where(p => p.GetParameters().Length == 2)
            .ToDictionary(p => p.GetParameters()[1].ParameterType.GenericTypeArguments[1]);
        private static readonly Dictionary<Type, MethodInfo> MaxMethodWith2Args = typeof(Enumerable).GetMethods()
          .Where(p => p.Name == nameof(Enumerable.Max))
          .Where(p => p.GetParameters().Length == 2)
          .ToDictionary(p => p.GetParameters()[1].ParameterType.GenericTypeArguments[1]);
        private static readonly Dictionary<Type, MethodInfo> AverageMethodWith2Args = typeof(Enumerable).GetMethods()
         .Where(p => p.Name == nameof(Enumerable.Average))
         .Where(p => p.GetParameters().Length == 2)
         .ToDictionary(p => p.GetParameters()[1].ParameterType.GenericTypeArguments[1]);

        private static readonly MethodInfo LongCountMethodWith1Args = typeof(Enumerable).GetMethods()
         .Where(p => p.Name == nameof(Enumerable.LongCount))
         .Where(p => p.GetParameters().Length == 1)
         .Single();

        public static MethodInfo GetSumAgg2<TSource, TResult>()
        {
            return SumMethodWith2Args[typeof(TResult)].MakeGenericMethod(typeof(TSource));
        }
        public static MethodInfo GetSumAgg2(Type sourceType, Type returnType)
        {
            return SumMethodWith2Args[returnType].MakeGenericMethod(sourceType);
        }
        public static MethodInfo GetMaxAgg2(Type sourceType, Type returnType)
        {
            return MaxMethodWith2Args[returnType].MakeGenericMethod(sourceType);
        }
        public static MethodInfo GetMinAgg2(Type sourceType, Type returnType)
        {
            return MinMethodWith2Args[returnType].MakeGenericMethod(sourceType);
        }
        public static MethodInfo GetAverageAgg2(Type sourceType, Type returnType)
        {
            return AverageMethodWith2Args[returnType].MakeGenericMethod(sourceType);
        }
        public static MethodInfo GetLongCount2(Type sourceType)
        {
            return LongCountMethodWith1Args.MakeGenericMethod(sourceType);
        }
        public static bool SupportAggValueType(Type valueType)
        {
            return SumMethodWith2Args.ContainsKey(valueType);
        }
    }
}
