using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace YS.Knife.Query
{
    internal class LambdaUtils
    {
        public static (LambdaExpression, Type) CreateValuePathLambda(Type fromType, List<ValuePath> paths, bool caseNullable)
        {
            var p = Expression.Parameter(fromType, "p");
            Expression currentExp = p;
            var currentType = fromType;
            foreach (var vp in paths)
            {
                var property = PropertyFinder.GetProertyOrField(currentType, vp.Name);
                currentExp = Expression.Property(currentExp, property);
                currentType = currentExp.Type;
            }
            //if (!EnumerableMethodFinder.SupportAggValueType(currentType))
            //{
            //    throw new InvalidOperationException($"can not support agg value type '{currentType.FullName}'");
            //}
            if (Nullable.GetUnderlyingType(currentType) == null && caseNullable)
            {
                currentType = typeof(Nullable<>).MakeGenericType(currentType);
                currentExp = Expression.Convert(currentExp, currentType);
            }
            return (Expression.Lambda(typeof(Func<,>).MakeGenericType(fromType, currentType), currentExp, p), currentType);
        }
    }
}
