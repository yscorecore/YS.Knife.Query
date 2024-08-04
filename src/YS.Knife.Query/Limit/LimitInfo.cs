using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YS.Knife.Query.Parser;

namespace YS.Knife.Query
{
    [Serializable]
    [TypeConverter(typeof(LimitIntoTypeConverter))]
    public record LimitInfo : ILimitInfo
    {
        public LimitInfo()
        {
        }
        public LimitInfo(int offset, int limit)
        {
            if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));
            if (limit < 0) throw new ArgumentOutOfRangeException(nameof(limit));
            this.Offset = offset;
            this.Limit = limit;
        }
        public int Offset { get; set; }
        public int Limit { get; set; }

        public static LimitInfo Parse(string limitStr) => Parse(limitStr, CultureInfo.CurrentCulture);
        public static LimitInfo Parse(string limitStr, CultureInfo currentCulture)
        {
            return new QueryExpressionParser(currentCulture).ParseLimitInfo(limitStr);
        }
        public override string ToString()
        {
            return $"{Offset},{Limit}";
        }
    }
}
