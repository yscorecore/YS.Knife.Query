using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YS.Knife.Query
{
    public record LimitInfo : ILimitInfo
    {
        public int Offset { get; set; }
        public int Limit { get; set; }
    }
}
