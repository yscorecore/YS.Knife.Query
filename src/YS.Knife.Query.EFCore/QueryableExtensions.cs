using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using static YS.Knife.Query.QueryableExtensions;

namespace YS.Knife.Query
{
    public static partial class QueryableExtensions2
    {
        public static Task<List<T>> QueryListAsync<T>(this IQueryable<T> source, LimitQueryInfo queryInfo, CancellationToken cancellationToken = default)
            where T : class, new()
        {
            return source.DoQuery(queryInfo).Skip(queryInfo.Offset).Take(queryInfo.Limit).ToListAsync(cancellationToken);
        }
        public static async Task<LimitList<R>> QueryLimitListAsync<T, R>(this IQueryable<T> source, LimitQueryInfo queryInfo, Func<IQueryable<T>, IQueryable<R>> mapper, CancellationToken cancellationToken = default)
            where T : class, new()
        {
            var data = await mapper(source.DoQuery(queryInfo).TryOrderByEntityKey()).Skip(queryInfo.Offset).Take(queryInfo.Limit + 1).ToListAsync(cancellationToken);
            return new LimitList<R>(data.Take(queryInfo.Limit), queryInfo.Offset, queryInfo.Limit, data.Count > queryInfo.Limit);
        }
        private static async Task<LimitList<R>> QueryLimitListAsync<T, R>(this IQueryable<T> source, ParsedQueryInfo<LimitQueryInfo> queryInfo, Func<IQueryable<T>, IQueryable<R>> mapper, CancellationToken cancellationToken = default)
            where T : class, new()
        {
            var data = await mapper(source.DoQuery(queryInfo).TryOrderByEntityKey()).Skip(queryInfo.RawQueryInfo.Offset).Take(queryInfo.RawQueryInfo.Limit + 1).ToListAsync(cancellationToken);
            return new LimitList<R>(data.Take(queryInfo.RawQueryInfo.Limit), queryInfo.RawQueryInfo.Offset, queryInfo.RawQueryInfo.Limit, data.Count > queryInfo.RawQueryInfo.Limit);
        }
        [Obsolete]
        public static Task<PagedList<T>> QueryPageAsync<T>(this IQueryable<T> source, LimitQueryInfo queryInfo, CancellationToken cancellationToken = default)
            where T : class, new()
        {
            return source.QueryPageAsync(queryInfo, p => p, cancellationToken);

        }

        public static async Task<PagedList<R>> QueryPageAsync<T, R>(this IQueryable<T> source, LimitQueryInfo queryInfo, Func<IQueryable<T>, IQueryable<R>> mapper, CancellationToken cancellationToken = default)
            where T : class, new()
            where R : class
        {
            _ = mapper ?? throw new ArgumentNullException(nameof(mapper));
            if (queryInfo.CountAll)
            {
                return await source.QueryPageInternalAsync(queryInfo, mapper, cancellationToken);
            }
            else
            {

                var parsed = YS.Knife.Query.QueryableExtensions.ParseQueryInfo(queryInfo);
                var limitList = await source.QueryLimitListAsync(parsed, mapper, cancellationToken);
                var aggResult = await source.DoFilter(parsed.Filter).QueryAgg(queryInfo, (p) => { return p.FirstOrDefaultAsync(cancellationToken); });
                return limitList.ToPagedList(aggResult);
            }

        }
        internal static async Task<PagedList<R>> QueryPageInternalAsync<T, R>(this IQueryable<T> source, LimitQueryInfo queryInfo, Func<IQueryable<T>, IQueryable<R>> mapper, CancellationToken cancellationToken = default)
            where T : class, new()
            where R : class
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            _ = queryInfo ?? throw new ArgumentNullException(nameof(queryInfo));
            _ = mapper ?? throw new ArgumentNullException(nameof(mapper));

            var parsed = ParseQueryInfo(queryInfo);
            var query = source.DoQuery(parsed);
            var aggResult = string.IsNullOrWhiteSpace(queryInfo.Agg) ? null : await source.DoFilter(parsed.Filter).QueryAgg(queryInfo, (p) => { return p.FirstOrDefaultAsync(cancellationToken); });
            if (queryInfo.Limit <= 0)
            {
                //only count all
                var totalCount = await query.LongCountAsync(cancellationToken);
                return new PagedList<R>(new List<R>(), queryInfo.Offset, queryInfo.Limit, totalCount, aggResult);
            }
            else
            {
                var data = await mapper(query.TryOrderByEntityKey().Skip(queryInfo.Offset).Take(queryInfo.Limit)).ToListAsync(cancellationToken);
                if (data.Count > 0 && data.Count < queryInfo.Limit)
                {
                    var totalCount = queryInfo.Offset + data.Count;
                    return new PagedList<R>(data, queryInfo.Offset, queryInfo.Limit, totalCount, aggResult);
                }
                else
                {
                    var totalCount = await query.LongCountAsync();
                    return new PagedList<R>(data, queryInfo.Offset, queryInfo.Limit, totalCount, aggResult);
                }
            }

        }
    }
}
