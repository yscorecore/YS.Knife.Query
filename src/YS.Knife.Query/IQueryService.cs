using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YS.Knife.Query
{
    public interface IQueryPageService<T>
    {
        Task<PagedList<T>> QueryPage(LimitQueryInfo queryInfo);
    }
    public interface IQueryLimitListService<T>
    {
        Task<LimitList<T>> QueryLimit(LimitQueryInfo queryInfo);
    }
    public static class QueryPageServiceExtensions
    {
        public static async Task<long> CountAsync<T>(this IQueryPageService<T> service, QueryInfo queryInfo)
        {
            var res = await service.QueryPage(queryInfo.AsLimit(0));
            return res.TotalCount;
        }
        public static async Task<List<T>> QueryListAsync<T>(this IQueryPageService<T> service, QueryInfo queryInfo, int limitCount = 10000)
        {
            var res = await service.QueryPage(queryInfo.AsLimit(limitCount));
            return res.Items;
        }
        public static async Task<T> QueryOneAsync<T>(this IQueryPageService<T> service, QueryInfo queryInfo)
        {
            var res = await service.QueryPage(queryInfo.AsLimit(1));
            return res.Items.FirstOrDefault();
        }
    }
    public static class QueryLimitListServiceExtensions
    {
        public static async Task<List<T>> QueryListAsync<T>(this IQueryLimitListService<T> service, QueryInfo queryInfo, int limitCount = 10000)
        {
            var res = await service.QueryLimit(queryInfo.AsLimit(limitCount));
            return res.Items;
        }
        public static async Task<T> QueryOneAsync<T>(this IQueryLimitListService<T> service, QueryInfo queryInfo)
        {
            var res = await service.QueryLimit(queryInfo.AsLimit(1));
            return res.Items.FirstOrDefault();
        }
    }
}
