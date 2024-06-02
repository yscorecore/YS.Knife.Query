using System;
using System.ComponentModel.DataAnnotations;
using YS.Knife.Query.Limit;

namespace YS.Knife.Query
{
    [Serializable]
    public record LimitQueryInfo : QueryInfo, ILimitInfo
    {
        [Range(0, int.MaxValue)]
        public int Offset { get; set; } = 0;
        [Range(0, 10000)]
        public int Limit { get; set; } = 10000;
    }
}
