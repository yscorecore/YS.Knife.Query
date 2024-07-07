using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using YS.Knife.Query.Filter;
using YS.Knife.Query.Limit;

namespace YS.Knife.Query
{
    public static class QueryableExtensions
    {
        
        public static IQueryable<T> DoQuery<T>(this IQueryable<T> source, QueryInfo queryInfo)
        {
            return source;
        }
       
        public static List<T> QueryList<T>(this IQueryable<T> source, LimitQueryInfo queryInfo)
        {
            return source.DoQuery(queryInfo).Skip(queryInfo.Offset).Take(queryInfo.Limit).ToList();
        }
        public static Task<List<T>> QueryListAsync<T>(this IQueryable<T> source, LimitQueryInfo queryInfo)
        {
            return Task.FromResult(source.QueryList(queryInfo));
        }

        public static LimitList<T> QueryLimitList<T>(this IQueryable<T> source, LimitQueryInfo queryInfo)
        {
            var data = source.DoQuery(queryInfo).Skip(queryInfo.Offset).Take(queryInfo.Limit + 1).ToList();
            return new LimitList<T>(data.Take(queryInfo.Limit), queryInfo.Offset, queryInfo.Limit, data.Count > queryInfo.Limit);
        }
        public static Task<LimitList<T>> QueryLimitListAsync<T>(this IQueryable<T> source, LimitQueryInfo queryInfo)
        {
            return Task.FromResult(source.QueryLimitList(queryInfo));
        }

        public static PagedList<T> QueryPage<T>(this IQueryable<T> source, LimitQueryInfo queryInfo)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            _ = queryInfo ?? throw new ArgumentNullException(nameof(queryInfo));
            var query = source.DoQuery(queryInfo);
            if (queryInfo.Limit == 0)
            {
                //only count all
                var totalCount = query.LongCount();
                return new PagedList<T>(new List<T>(), queryInfo.Offset, queryInfo.Limit, totalCount);
            }
            else
            {
                var data = query.Skip(queryInfo.Offset).Take(queryInfo.Limit).ToList();
                if (data.Count < queryInfo.Limit)
                {
                    var totalCount = queryInfo.Offset + data.Count;
                    return new PagedList<T>(data, queryInfo.Offset, queryInfo.Limit, totalCount);
                }
                else
                {
                    var totalCount = query.LongCount();
                    return new PagedList<T>(data, queryInfo.Offset, queryInfo.Limit, totalCount);
                }
            }
        }
        public static Task<PagedList<T>> QueryPageAsync<T>(this IQueryable<T> source, LimitQueryInfo queryInfo)
        {
             return Task.FromResult(QueryPage(source, queryInfo));
        }

        //public static PagedList<T,S> QueryPage<T,S>(this IQueryable<T> source, LimitQueryInfo queryInfo)
        //{
           
        //}


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
        public static IQueryable<R> WhereItemsOr<T, R>(this IQueryable<R> query, IEnumerable<T> source, Expression<Func<R, T, bool>> predicate)
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
