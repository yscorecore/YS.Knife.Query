using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using YS.Knife.Query.Parser;

namespace YS.Knife.Query
{
    public class SelectInfo
    {
        public List<SelectItem> Items { get; set; }

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
