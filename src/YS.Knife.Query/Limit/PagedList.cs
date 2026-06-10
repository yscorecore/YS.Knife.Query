using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
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
            this.HasNext = this.TotalCount > this.Offset + this.Limit;
        }
        public PagedList(IEnumerable<T> limitListData, int offset, int limit, long? totalCount, bool hasNext, IDictionary<string, object> aggs = null)
        {
            this.Limit = limit;
            this.Offset = offset;
            this.TotalCount = totalCount;
            this.Items = limitListData?.ToList() ?? new List<T>();
            this.Aggs = aggs == null ? null : new Dictionary<string, object>(aggs);
            this.HasNext = hasNext;
        }
        [Required]
        [JsonPropertyOrder(0)]
        public int Offset { get; set; }

        [Required]
        [JsonPropertyOrder(1)]
        public int Limit { get; set; }
        [Required]
        [JsonPropertyOrder(2)]
        public bool HasNext { get; set; }

        [JsonPropertyOrder(3)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public long? TotalCount { get; set; }
        [JsonPropertyOrder(5)]
        [Required]
        public List<T> Items { get; set; }

        [JsonPropertyOrder(4)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string, object> Aggs { get; set; }
    }


}
