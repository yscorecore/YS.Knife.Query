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
        public static async Task<LimitList<T>> QueryLimitListAsync<T>(this IQueryable<T> source, LimitQueryInfo queryInfo, CancellationToken cancellationToken = default)
            where T : class, new()
        {
            var data = await source.DoQuery(queryInfo).TryOrderByEntityKey().Skip(queryInfo.Offset).Take(queryInfo.Limit + 1).ToListAsync(cancellationToken);
            return new LimitList<T>(data.Take(queryInfo.Limit), queryInfo.Offset, queryInfo.Limit, data.Count > queryInfo.Limit);
        }
        private static async Task<LimitList<T>> QueryLimitListAsync<T>(this IQueryable<T> source, ParsedQueryInfo<LimitQueryInfo> queryInfo, CancellationToken cancellationToken = default)
            where T : class, new()
        {
            var data = await source.DoQuery(queryInfo).TryOrderByEntityKey().Skip(queryInfo.RawQueryInfo.Offset).Take(queryInfo.RawQueryInfo.Limit + 1).ToListAsync(cancellationToken);
            return new LimitList<T>(data.Take(queryInfo.RawQueryInfo.Limit), queryInfo.RawQueryInfo.Offset, queryInfo.RawQueryInfo.Limit, data.Count > queryInfo.RawQueryInfo.Limit);
        }
        public static async Task<PagedList<T>> QueryPageAsync<T>(this IQueryable<T> source, LimitQueryInfo queryInfo, CancellationToken cancellationToken = default)
            where T : class, new()
        {
            if (queryInfo.CountAll)
            {
                return await source.QueryPageInternalAsync(queryInfo, cancellationToken);
            }
            else
            {

                var parsed = YS.Knife.Query.QueryableExtensions.ParseQueryInfo(queryInfo);
                var limitList = await source.QueryLimitListAsync(parsed, cancellationToken);
                var aggResult = await source.DoFilter(parsed.Filter).QueryAgg(queryInfo, (p) => { return p.FirstOrDefaultAsync(cancellationToken); });
                return limitList.ToPagedList(aggResult);
            }

        }
        public static async Task<PagedList<T>> QueryPageInternalAsync<T>(this IQueryable<T> source, LimitQueryInfo queryInfo, CancellationToken cancellationToken = default)
            where T : class, new()
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            _ = queryInfo ?? throw new ArgumentNullException(nameof(queryInfo));

            var parsed = ParseQueryInfo(queryInfo);
            var query = source.DoQuery(parsed);
            var aggResult = await source.DoFilter(parsed.Filter).QueryAgg(queryInfo, (p) => { return p.FirstOrDefaultAsync(cancellationToken); });
            if (queryInfo.Limit <= 0)
            {
                //only count all
                var totalCount = await query.LongCountAsync(cancellationToken);
                return new PagedList<T>(new List<T>(), queryInfo.Offset, queryInfo.Limit, totalCount, aggResult);
            }
            else
            {
                var data = await query.TryOrderByEntityKey().Skip(queryInfo.Offset).Take(queryInfo.Limit).ToListAsync(cancellationToken);
                if (data.Count > 0 && data.Count < queryInfo.Limit)
                {
                    var totalCount = queryInfo.Offset + data.Count;
                    return new PagedList<T>(data, queryInfo.Offset, queryInfo.Limit, totalCount, aggResult);
                }
                else
                {
                    var totalCount = await query.LongCountAsync();
                    return new PagedList<T>(data, queryInfo.Offset, queryInfo.Limit, totalCount, aggResult);
                }
            }

        }
    }
}
