using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using YS.Knife.Query.Parser;

namespace YS.Knife.Query
{
    [TypeConverter(typeof(AggInfoTypeConverter))]
    [Serializable]
    public class AggInfo
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

        public void Add(AggItem item)
        {
            this.Items.Add(item);
        }
        public bool HasItems()
        {
            return this.Items?.Count > 0;
        }
        public static AggInfo Parse(string orderText)
        {
            return Parse(orderText, CultureInfo.CurrentCulture);
        }
        public static AggInfo Parse(string orderText, CultureInfo cultureInfo)
        {
            return new QueryExpressionParser(cultureInfo).ParseAggInfo(orderText);
        }
        public override string ToString()
        {
            return string.Join(",", this.Items.TrimNotNull());
        }
    }
}
