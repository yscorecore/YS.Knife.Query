using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [Required]
        public bool HasNext
        {
            get
            {
                return this.TotalCount > this.Offset + this.Limit;
            }
        }
        [Required]
        public int Limit { get; set; }

        [Required]
        public List<T> Items { get; set; } 
        [Required]
        public int Offset { get; set; }

        [Required]
        public long TotalCount { get; set; }
        public Dictionary<string, object> Aggs { get; }
    }


}
