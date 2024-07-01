using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YS.Knife.Query
{
    public class OrderByItem
    {
        public List<ValuePath> NavigatePaths { get; set; }
        public OrderByType OrderByType { get; set; }

        public override string ToString()
        {
            var paths = NavigatePaths.TrimNotNull();
            if (paths.Count() > 0)
            {
                string path = string.Join(".", paths);
                return OrderByType == OrderByType.Desc ? $"{path}.desc()" : $"{path}.asc()";
            }
            return string.Empty;
        }
        internal static OrderByItem FromValuePaths(List<ValuePath> paths)
        {
            var last = paths?.LastOrDefault();

            if (last != null && last.IsFunction)
            {
                if (string.Equals(last.Name, nameof(OrderByType.Desc), StringComparison.InvariantCultureIgnoreCase))
                {
                    return new OrderByItem
                    {
                        NavigatePaths = paths.SkipLast(1).ToList(),
                        OrderByType = OrderByType.Desc
                    };
                }
                else if (string.Equals(last.Name, nameof(OrderByType.Asc), StringComparison.InvariantCultureIgnoreCase))
                {
                    return new OrderByItem
                    {
                        NavigatePaths = paths.SkipLast(1).ToList(),
                        OrderByType = OrderByType.Asc
                    };
                }
            }
            return new OrderByItem
            {
                NavigatePaths = paths,
                OrderByType = OrderByType.Asc
            };
        }
    }
}

