using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using YS.Knife.Query.Parser;
using static System.Linq.AggExtensions;

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
            var data = source.DoQuery(queryInfo).Skip(queryInfo.Offset).Take(queryInfo.Limit + 1).ToList();
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
        internal static Task<Dictionary<string, object>> QueryAggAsync<T>(this IQueryable<T> source, LimitQueryInfo limitQueryInfo, Func<IQueryable<TempRecord>, Task<TempRecord>> firstOrDefaultFun)
        {
            var aggInfo = ParseAgg(limitQueryInfo.Agg);
            return DoAgg(source, aggInfo,firstOrDefaultFun);
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
                    return source.DoAggAsync(agg,firstOrDefaultFun);
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
                var data = query.Skip(queryInfo.Offset).Take(queryInfo.Limit).ToList();
                if (data.Count < queryInfo.Limit)
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
        public static IQueryable<R> WhereItemsAnd<T, R>(this IQueryable<R> query, IEnumerable<T> source, Expression<Func<T, R, bool>> predicate)
        {
            Expression expression = Expression.Constant(true);
            foreach (var item in source)
            {
                var parameterReplacer = new ParameterReplacerVisitor(predicate.Parameters[1], Expression.Constant(item));
                var segment = parameterReplacer.Visit(predicate.Body);
                expression = Expression.AndAlso(expression, segment);
            }
            var lambda = Expression.Lambda<Func<R, bool>>(expression, predicate.Parameters[0]);
            return query.Where(lambda);
        }
        internal static IQueryable<R> WhereItemsOr<T, R>(this IQueryable<R> query, IEnumerable<T> source, Expression<Func<R, T, bool>> predicate)
        {
            Expression expression = Expression.Constant(false);
            foreach (var item in source)
            {
                var parameterReplacer = new ParameterReplacerVisitor(predicate.Parameters[1], Expression.Constant(item));
                var segment = parameterReplacer.Visit(predicate.Body);
                expression = Expression.OrElse(expression, segment);
            }
            var lambda = Expression.Lambda<Func<R, bool>>(expression, predicate.Parameters[0]);
            return query.Where(lambda);
        }


        class ParameterReplacerVisitor : ExpressionVisitor
        {
            private readonly Expression _oldParameter;
            private readonly Expression _newParameter;

            public ParameterReplacerVisitor(Expression oldParameter, Expression newParameter)
            {
                _oldParameter = oldParameter;
                _newParameter = newParameter;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                return node == _oldParameter ? _newParameter : base.VisitParameter(node);
            }
        }
    }
}
