using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using YS.Knife.Query.Parser;

namespace YS.Knife.Query
{
    [TypeConverter(typeof(SelectInfoTypeConverter))]
    [Serializable]
    public class SelectInfo
    {
        public SelectInfo()
        {

        }
        public SelectInfo(params SelectItem[] selectItems)
        {
            var items = (selectItems ?? Enumerable.Empty<SelectItem>()).Where(p => p != null);
            this.Items.AddRange(items);
        }
        public List<SelectItem> Items { get; set; } = new List<SelectItem>();

        public override string ToString()
        {
            if (Items == null) return string.Empty;
            return string.Join(',', Items.TrimNotNull().Select(p => p.ToString()));
        }

        public static SelectInfo Parse(string text)
        {
            return Parse(text, CultureInfo.CurrentCulture);
        }
        public static SelectInfo Parse(string text, CultureInfo culture)
        {
            return new QueryExpressionParser(culture).ParseSelectInfo(text);
        }

    }
}
