using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YS.Knife.Query
{
    public class AggItem
    {
        public AggType AggType { get; set; }
        public List<ValuePath> NavigatePaths { get; set; }
        public string AggName { get; set; }
    }
}
