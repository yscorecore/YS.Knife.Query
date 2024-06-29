using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YS.Knife.Query
{
    public enum AggType
    {
        Sum = 0,
        Min = 1,
        Max = 2,
        Avg = 3,
        Count = 100,
        DistinctCount = 200
    }
}
