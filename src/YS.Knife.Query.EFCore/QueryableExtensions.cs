using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;
using static System.Linq.AggExtensions;

namespace YS.Knife.Query
{
    public static partial class QueryableExtensions
    {
        public static Task<List<T>> QueryListAsync<T>(this IQueryable<T> source, LimitQueryInfo queryInfo)
            where T : class, new()
        {
            return source.DoQuery(queryInfo).Skip(queryInfo.Offset).Take(queryInfo.Limit).ToListAsync();
        }
        public static async Task<LimitList<T>> QueryLimitListAsync<T>(this IQueryable<T> source, LimitQueryInfo queryInfo)
            where T : class, new()
        {
            var data = await source.DoQuery(queryInfo).Skip(queryInfo.Offset).Take(queryInfo.Limit + 1).ToListAsync();
            return new LimitList<T>(data.Take(queryInfo.Limit), queryInfo.Offset, queryInfo.Limit, data.Count > queryInfo.Limit);
        }
        public static async Task<PagedList<T>> QueryPageAsync<T>(this IQueryable<T> source, LimitQueryInfo queryInfo)
            where T : class, new()
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            _ = queryInfo ?? throw new ArgumentNullException(nameof(queryInfo));
            var query = source.DoQuery(queryInfo);
            var aggResult = await  source.QueryAggAsync(queryInfo,(p)=> { return p.FirstOrDefaultAsync(); });
            if (queryInfo.Limit <= 0)
            {
                //only count all
                var totalCount = await query.LongCountAsync();
                return new PagedList<T>(new List<T>(), queryInfo.Offset, queryInfo.Limit, totalCount, aggResult);
            }
            else
            {
                var data = await query.Skip(queryInfo.Offset).Take(queryInfo.Limit).ToListAsync();
                if (data.Count < queryInfo.Limit)
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
