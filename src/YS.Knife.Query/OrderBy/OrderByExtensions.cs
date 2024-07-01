using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace YS.Knife.Query
{
    public static class OrderByExtensions
    {
        public static IQueryable<T> DoOrderBy<T>(this IQueryable<T> source, OrderByInfo orderInfo)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            return source.DoOrderBy(orderInfo?.Items ?? Enumerable.Empty<OrderByItem>());
        }


        private static IQueryable<TSource> DoOrderBy<TSource>(this IQueryable<TSource> source,
                params (LambdaExpression KeySelector, OrderByType OrderType)[] orderByRules)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            IOrderedQueryable<TSource> result = null;
            foreach (var (lambda, type) in orderByRules)
            {
                if (lambda == null) continue;

                if (type == OrderByType.Asc)
                {
                    //顺序
                    if (result != null)
                    {
                        result = result.ThenAsc(lambda);
                    }
                    else
                    {
                        result = source.Asc(lambda);
                    }
                }
                else
                {
                    if (result != null)
                    {
                        result = result.ThenDesc(lambda);
                    }
                    else
                    {
                        result = source.Desc(lambda);
                    }
                }
            }

            return result ?? source;
        }
        private static IQueryable<T> DoOrderBy<T>(this IQueryable<T> source, IEnumerable<OrderByItem> orderItems)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            var lambdas = orderItems
                .TrimNotNull()
                .Select(item => (CreateKeySelectorLambda<T>(item.NavigatePaths), item.OrderByType))
                .ToArray();

            return source.DoOrderBy(lambdas);
        }



        private static LambdaExpression CreateKeySelectorLambda<TSource>(List<ValuePath> valuePaths)
        {
            var (lambdaExp, _) = LambdaUtils.CreateValuePathLambda(typeof(TSource), valuePaths, false);
            return lambdaExp;
        }

        private static IOrderedQueryable<T> Asc<T>(this IQueryable<T> source, LambdaExpression keySelector)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            _ = keySelector ?? throw new ArgumentNullException(nameof(keySelector));

            return DoOrder(source, nameof(Queryable.OrderBy), keySelector);
        }

        private static IOrderedQueryable<T> ThenAsc<T>(this IQueryable<T> source, LambdaExpression keySelector)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            _ = keySelector ?? throw new ArgumentNullException(nameof(keySelector));

            return DoOrder(source, nameof(Queryable.ThenBy), keySelector);
        }

        private static IOrderedQueryable<T> Desc<T>(this IQueryable<T> source, LambdaExpression keySelector)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            _ = keySelector ?? throw new ArgumentNullException(nameof(keySelector));
            return DoOrder(source, nameof(Queryable.OrderByDescending), keySelector);
        }

        private static IOrderedQueryable<T> ThenDesc<T>(this IQueryable<T> source, LambdaExpression keySelector)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            _ = keySelector ?? throw new ArgumentNullException(nameof(keySelector));
            return DoOrder(source, nameof(Queryable.ThenByDescending), keySelector);
        }

        private static IOrderedQueryable<TSource> DoOrder<TSource>(IQueryable<TSource> source, string methodName,
            LambdaExpression keySelector)
        {
            var types = new Type[] { typeof(TSource), keySelector.ReturnType };
            Expression expr = Expression.Call(typeof(Queryable),
                methodName, types, source.Expression, keySelector);
            return source.Provider.CreateQuery<TSource>(expr) as IOrderedQueryable<TSource>;
        }
    }
}
