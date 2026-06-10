using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using YS.Knife.Query.Parser;
using static System.Linq.AggExtensions;

[assembly: InternalsVisibleTo("YS.Knife.Query.EFCore")]

namespace YS.Knife.Query
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> DoQuery<T>(this IQueryable<T> source, QueryInfo queryInfo)
           where T : class, new()
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            if (queryInfo == null) return source;
            var filter = ParseFilter(queryInfo.Filter);
            var orderBy = ParseOrderBy(queryInfo.OrderBy);
            var select = ParseSelect(queryInfo.Select);
            var result = source;
            result = Filter(result, filter);
            result = OrderBy(result, orderBy);
            result = Select(result, select);
            if (queryInfo.Distinct)
            {
                result = result.Distinct();
            }
            return result;
            static FilterInfo ParseFilter(string filter)
            {
                try
                {
                    return FilterInfo.Parse(filter);
                }
                catch (ParseException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new ParseException("parse filter error.", ex);
                }
            }
            static OrderByInfo ParseOrderBy(string orderby)
            {
                try
                {
                    return OrderByInfo.Parse(orderby);
                }
                catch (ParseException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new ParseException("parse order error", ex);
                }
            }
            static SelectInfo ParseSelect(string select)
            {
                try
                {
                    return SelectInfo.Parse(select);
                }
                catch (ParseException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new ParseException("passe select error.", ex);
                }
            }
            static IQueryable<T> Filter(IQueryable<T> source, FilterInfo filter)
            {
                try
                {
                    return source.DoFilter(filter);
                }
                catch (Exception)
                {

                    throw;
                }
            }
            static IQueryable<T> OrderBy(IQueryable<T> source, OrderByInfo orderBy)
            {
                try
                {
                    return source.DoOrderBy(orderBy);
                }
                catch (Exception)
                {

                    throw;
                }
            }
            static IQueryable<T> Select(IQueryable<T> source, SelectInfo select)
            {
                try
                {
                    return source.DoSelect(select);
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        public static List<T> QueryList<T>(this IQueryable<T> source, LimitQueryInfo queryInfo)
            where T : class, new()
        {
            return source.DoQuery(queryInfo).Skip(queryInfo.Offset).Take(queryInfo.Limit).ToList();
        }

        public static LimitList<T> QueryLimitList<T>(this IQueryable<T> source, LimitQueryInfo queryInfo)
            where T : class, new()
        {
            var data = source.DoQuery(queryInfo).TryOrderByEntityKey().Skip(queryInfo.Offset).Take(queryInfo.Limit + 1).ToList();
            return new LimitList<T>(data.Take(queryInfo.Limit), queryInfo.Offset, queryInfo.Limit, data.Count > queryInfo.Limit);
        }

        private static Dictionary<string, object> QueryAgg<T>(this IQueryable<T> source, LimitQueryInfo limitQueryInfo)
        {
            var aggInfo = ParseAgg(limitQueryInfo.Agg);
            return DoAgg(source, aggInfo);
            static AggInfo ParseAgg(string agg)
            {
                try
                {
                    return AggInfo.Parse(agg);
                }
                catch (ParseException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new ParseException("parse agg error.", ex);
                }
            }
            static Dictionary<string, object> DoAgg(IQueryable<T> source, AggInfo agg)
            {
                try
                {
                    return source.DoAgg(agg);
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }
        internal static Task<Dictionary<string, object>> QueryAgg<T>(this IQueryable<T> source, LimitQueryInfo limitQueryInfo, Func<IQueryable<TempRecord>, Task<TempRecord>> firstOrDefaultFun)
        {
            var aggInfo = ParseAgg(limitQueryInfo.Agg);
            return DoAgg(source, aggInfo, firstOrDefaultFun);
            static AggInfo ParseAgg(string agg)
            {
                try
                {
                    return AggInfo.Parse(agg);
                }
                catch (ParseException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new ParseException("parse agg error.", ex);
                }
            }
            static Task<Dictionary<string, object>> DoAgg(IQueryable<T> source, AggInfo agg, Func<IQueryable<TempRecord>, Task<TempRecord>> firstOrDefaultFun)
            {
                try
                {
                    return source.DoAggAsync(agg, firstOrDefaultFun);
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }
        public static PagedList<T> QueryPage<T>(this IQueryable<T> source, LimitQueryInfo queryInfo)
           where T : class, new()
        {
            if (queryInfo.CountAll)
            {
                return source.QueryPageInternal(queryInfo);
            }
            else
            {
                var limitList = source.QueryLimitList(queryInfo);
                var aggResult = source.QueryAgg(queryInfo);
                return limitList.ToPagedList(aggResult);
            }
        }
        public static PagedList<T> QueryPageInternal<T>(this IQueryable<T> source, LimitQueryInfo queryInfo)
            where T : class, new()
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            _ = queryInfo ?? throw new ArgumentNullException(nameof(queryInfo));
            var query = source.DoQuery(queryInfo);
            var aggResult = source.QueryAgg(queryInfo);
            if (queryInfo.Limit <= 0)
            {
                //only count all
                var totalCount = query.LongCount();
                return new PagedList<T>(new List<T>(), queryInfo.Offset, queryInfo.Limit, totalCount, aggResult);
            }
            else
            {
                var data = query.TryOrderByEntityKey().Skip(queryInfo.Offset).Take(queryInfo.Limit).ToList();
                if (data.Count > 0 && data.Count < queryInfo.Limit)
                {
                    var totalCount = queryInfo.Offset + data.Count;
                    return new PagedList<T>(data, queryInfo.Offset, queryInfo.Limit, totalCount, aggResult);
                }
                else
                {
                    var totalCount = query.LongCount();
                    return new PagedList<T>(data, queryInfo.Offset, queryInfo.Limit, totalCount, aggResult);
                }
            }
        }

    }

    internal static class QueryableOrderByExtension
    {
        static readonly string[] SkipMethodNames = new string[]
            {
                nameof(Queryable.Where),
                nameof(Queryable.Select),
                nameof(Queryable.Skip),
                nameof(Queryable.SelectMany),
                nameof(Queryable.Take),
                nameof(Queryable.Distinct),
                nameof(Queryable.DistinctBy),
                nameof(Queryable.GroupBy)
            };
        static readonly string[] OrderByMethods = new string[] {
                 nameof(Queryable.OrderBy),
                 nameof(Queryable.OrderByDescending),
                 nameof(Queryable.ThenBy),
                 nameof(Queryable.ThenByDescending),

            };
        public static IQueryable<T> TryOrderByEntityKey<T>(this IQueryable<T> query)
        {
            Stack<MethodCallExpression> methodStacks = new Stack<MethodCallExpression>();
            Expression expression = query.Expression;
            while (expression is MethodCallExpression methodCall && methodCall.Method.DeclaringType == typeof(Queryable) && SkipMethodNames.Contains(methodCall.Method.Name))
            {
                methodStacks.Push(methodCall);
                expression = methodCall.Arguments[0];
            }
            //找到原始的类型
            var itemType = expression.Type.GetGenericArguments()[0];
            var keyProperty = FindKeyProperty(itemType);
            if (keyProperty == null || HasOrderByKey(expression, keyProperty))
            {
                return query;
            }
            else
            {
                var orderByMethod = typeof(IOrderedQueryable<>).MakeGenericType(itemType) == expression.Type ?
                      GetThenByMethod(itemType, keyProperty.PropertyType) : GetOrderByMethod(itemType, keyProperty.PropertyType);

                expression = Expression.Call(null, orderByMethod, expression, BuildKeySelect(itemType, keyProperty));
                while (methodStacks.Any())
                {
                    var methodCall = methodStacks.Pop();
                    var arguments = methodCall.Arguments.ToArray();
                    arguments[0] = expression;
                    expression = Expression.Call(null, methodCall.Method, arguments);
                }
                return query.Provider.CreateQuery<T>(expression);
            }

        }
        private static MethodInfo OrderByGenericMethod = typeof(Queryable).GetMethods(BindingFlags.Static | BindingFlags.Public).Where(p => p.Name == nameof(Queryable.OrderBy) && p.GetParameters().Length == 2).SingleOrDefault();
        private static MethodInfo ThenByGenericMethod = typeof(Queryable).GetMethods(BindingFlags.Static | BindingFlags.Public).Where(p => p.Name == nameof(Queryable.ThenBy) && p.GetParameters().Length == 2).SingleOrDefault();

        private static MethodInfo GetOrderByMethod(Type type, Type keyType)
        {
            return OrderByGenericMethod.MakeGenericMethod(type, keyType);
        }
        private static MethodInfo GetThenByMethod(Type type, Type keyType)
        {
            return ThenByGenericMethod.MakeGenericMethod(type, keyType); ;
        }
        private static Expression BuildKeySelect(Type type, PropertyInfo property)
        {
            var p = Expression.Parameter(type, "p");
            return Expression.Quote(Expression.Lambda(Expression.Property(p, property), p));
        }
        private static bool HasOrderByKey(Expression expression, PropertyInfo property)
        {
            while (expression is MethodCallExpression methodCall && methodCall.Method.DeclaringType == typeof(Queryable) && OrderByMethods.Contains(methodCall.Method.Name))
            {
                if (LambdaContainsProperty(methodCall.Arguments[1], property))
                {
                    return true;
                }
                expression = methodCall.Arguments[0];
            }
            return false;
        }
        private static bool LambdaContainsProperty(Expression expression, PropertyInfo property)
        {
            var quoteExpression = expression as UnaryExpression;
            var lambdaExpression = quoteExpression?.Operand as LambdaExpression;
            var lambdaBody = lambdaExpression?.Body as MemberExpression;
            return lambdaBody?.Member == property;
        }
        private static PropertyInfo FindKeyProperty(Type type)
        {
            var allProperties = type.GetProperties();
            return
                allProperties.Where(p => Attribute.IsDefined(p, typeof(KeyAttribute))).FirstOrDefault()
                ?? allProperties.Where(p => p.Name.Equals("Id", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault()
                ?? allProperties.Where(p => p.Name.Equals($"{type.Name}Id", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
        }
    }
}
