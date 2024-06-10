using System;
using System.ComponentModel.DataAnnotations;

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
        [MaxLength(1024)]
        public string Agg { get; set; }

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
}
