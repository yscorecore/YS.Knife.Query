using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace YS.Knife.Query
{
    [Serializable]
    public record QueryInfo
    {
        [MaxLength(4096)]
        public string Filter { get; set; }
        [MaxLength(1024)]
        public string OrderBy { get; set; }
        [MaxLength(1024)]
        public string Select { get; set; }

        public bool Distinct { get; set; }

        public LimitQueryInfo AsLimit(int limitCount = 10000)
        {
            return new LimitQueryInfo
            {
                Filter = this.Filter,
                Limit = limitCount,
                Offset = 0,
                OrderBy = this.OrderBy,
                Select = this.Select
            };
        }
    }

    public static class QueryInfoExtensions
    {
        public static T AppendFilter<T>(this T query, string filter)
            where T : QueryInfo
        {
            return query with { Filter = CombinFilter(query.Filter, filter) };
        }
        public static T PrependFilter<T>(this T query, string filter)
           where T : QueryInfo
        {
            return query with { Filter = CombinFilter(filter, query.Filter) };
        }
        private static string CombinFilter(string filter1, string filter2)
        {
            if (string.IsNullOrEmpty(filter1))
            {
                return filter2;
            }
            if (string.IsNullOrEmpty(filter2))
            {
                return filter1;
            }
            return $"({filter1}) {FilterInfo.Operator_And} ({filter2})";
        }
    }
}
