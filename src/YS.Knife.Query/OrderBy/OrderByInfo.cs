using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using YS.Knife.Query.Parser;

namespace YS.Knife.Query
{
    [TypeConverter(typeof(OrderByInfoTypeConverter))]
    [Serializable]
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

        public bool HasItems()
        {
            return this.Items != null && this.Items.Count > 0;
        }

        public override string ToString()
        {
            return string.Join(",", this.Items.TrimNotNull());
        }

        public OrderByInfo Add(OrderByItem orderItem)
        {
            this.Items.Add(orderItem);
            return this;
        }
        public static OrderByInfo Parse(string orderText)
        {
            return Parse(orderText, CultureInfo.CurrentCulture);
        }
        public static OrderByInfo Parse(string orderText, CultureInfo cultureInfo)
        {
            return new QueryExpressionParser(cultureInfo).ParseOrderBy(orderText);
        }
    }
}
