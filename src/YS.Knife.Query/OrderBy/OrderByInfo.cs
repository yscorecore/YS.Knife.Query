using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YS.Knife.Query
{
    public class OrderByInfo
    {
        public OrderByInfo()
        {
        }

        public OrderByInfo(params OrderByItem[] orderItems)
        {
            var items = (orderItems ?? Enumerable.Empty<OrderByItem>()).Where(p => p != null);
            this.Items.AddRange(items);
        }

        public List<OrderByItem> Items { get; set; } = new List<OrderByItem>();



        public static OrderByInfo Create(OrderByItem orderItem)
        {
            return new OrderByInfo(new OrderByItem[] { orderItem });
        }
        public static OrderByInfo Create(params OrderByItem[] orderItems)
        {
            return new OrderByInfo(orderItems);
        }
    }
}
