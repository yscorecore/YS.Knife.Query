using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YS.Knife.Query
{
    public record LimitInfo : ILimitInfo
    {
        public LimitInfo()
        {
        }
        public LimitInfo(int offset, int limit)
        {
            if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));
            if (limit < 0) throw new ArgumentOutOfRangeException(nameof(limit));
            this.Offset = offset;
            this.Limit = limit;
        }
        public int Offset { get; set; }
        public int Limit { get; set; }
    }
}
