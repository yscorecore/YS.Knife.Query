using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YS.Knife.Query.Limit
{
    [Serializable]
    public record PagedList<T> : IPagedList<T>
    {
        public PagedList()
        {
        }
        public PagedList(IEnumerable<T> limitListData, int offset, int limit, long totalCount)
        {
            this.Limit = limit;
            this.Offset = offset;
            this.TotalCount = totalCount;
            this.Items = limitListData?.ToList() ?? new List<T>();
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

    }

    [Serializable]
    public record PagedList<T, S> : PagedList<T>, IPagedList<T, S>
    {
        public S Summary { get; }
    }
}
