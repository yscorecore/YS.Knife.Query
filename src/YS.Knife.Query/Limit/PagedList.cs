using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YS.Knife.Query
{
    [Serializable]
    public record PagedList<T> : IPagedList<T>, IAggData
    {
        public PagedList()
        {
        }
        public PagedList(IEnumerable<T> limitListData, int offset, int limit, long totalCount)
            : this(limitListData, offset, limit, totalCount, null)
        {

        }
        public PagedList(IEnumerable<T> limitListData, int offset, int limit, long totalCount, IDictionary<string, object> aggs)
        {
            this.Limit = limit;
            this.Offset = offset;
            this.TotalCount = totalCount;
            this.Items = limitListData?.ToList() ?? new List<T>();
            this.Aggs = aggs == null ? null : new Dictionary<string, object>(aggs);
        }

        public bool HasNext
        {
            get
            {
                return this.TotalCount > this.Offset + this.Limit;
            }
        }

        public int Limit { get; set; }


        public List<T> Items { get; set; }

        public int Offset { get; set; }


        public long TotalCount { get; set; }
        public Dictionary<string, object> Aggs { get; }
    }


}
