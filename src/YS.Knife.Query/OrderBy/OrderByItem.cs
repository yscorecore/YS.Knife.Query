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
    }
}

