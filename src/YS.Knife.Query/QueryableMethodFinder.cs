using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace YS.Knife.Query
{
    internal class QueryableMethodFinder
    {
        public static readonly MethodInfo QuerySelectMethod = typeof(Queryable).GetMethods()
           .Where(p => p.Name == nameof(Queryable.Select))
           .Where(p => p.GetParameters().Length == 2)
           .Single(p =>
               p.GetParameters().Last().ParameterType.GetGenericArguments().First().GetGenericTypeDefinition() ==
               typeof(Func<,>));


        public static MethodInfo GetQuerybleSelect<TSource, TResult>()
        {
            return QuerySelectMethod.MakeGenericMethod(typeof(TSource), typeof(TResult));
        }

    }
}
