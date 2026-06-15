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
        public FilterInfo Filter { get; set; }
        public override string ToString()
        {
            var sb = new StringBuilder();
            if (NavigatePaths != null)
            {
                sb.Append(string.Join(".", NavigatePaths));
            }
            var filterText = this.Filter?.ToString() ?? string.Empty;
            sb.Append(string.Format($".{AggType.ToString().ToLowerInvariant()}({filterText})"));
            if (!string.IsNullOrEmpty(AggName))
            {
                sb.Append($".as({AggName})");
            }
            return sb.ToString();
        }
    }
}
