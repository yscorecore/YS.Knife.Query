using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YS.Knife.Query
{
    public record AggInfo
    {
        public AggInfo()
        {
        }

        public AggInfo(params AggItem[] aggItems)
        {
            var items = (aggItems ?? Enumerable.Empty<AggItem>()).Where(p => p != null);
            this.Items.AddRange(items);
        }

        public List<AggItem> Items { get; set; } = new List<AggItem>();

    }
}
