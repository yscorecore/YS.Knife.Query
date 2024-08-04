using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YS.Knife.Query.Parser
{
    internal partial class QueryExpressionParser
    {
        public CultureInfo CurrentCulture { get; }
        public QueryExpressionParser(CultureInfo cultureInfo)
        {
            this.CurrentCulture = cultureInfo ?? throw new ArgumentNullException(nameof(cultureInfo));
        }
        public FilterInfo ParseFilter(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) { return null; }
            var context = new ParseContext(text, this.CurrentCulture);
            var filterInfo = context.ParseFilterInfo();
            context.SkipWhiteSpace();
            if (context.NotEnd())
            {
                throw ParseErrors.InvalidText(context);
            }
            return filterInfo;
        }
        public OrderByInfo ParseOrderBy(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;
            var context = new ParseContext(text, this.CurrentCulture);
            OrderByInfo orderInfo = context.ParseOrderByInfo();
            context.SkipWhiteSpace();
            if (context.NotEnd())
            {
                throw ParseErrors.InvalidText(context);
            }
            return orderInfo;
        }
        public ValueInfo ParseValue(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;
            var context = new ParseContext(text, this.CurrentCulture);
            context.SkipWhiteSpace();
            var value = context.ParseValueInfo();
            if (context.NotEnd())
            {
                throw ParseErrors.InvalidText(context);
            }
            return value;
        }

        public SelectInfo ParseSelectInfo(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;
            var context = new ParseContext(text, this.CurrentCulture);
            SelectInfo selectInfo = context.ParseSelectInfo();
            context.SkipWhiteSpace();
            if (context.NotEnd())
            {
                throw ParseErrors.InvalidText(context);
            }
            return selectInfo;
        }
        public LimitInfo ParseLimitInfo(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;
            var context = new ParseContext(text, this.CurrentCulture);
            LimitInfo limitInfo = context.ParseLimitInfo();
            context.SkipWhiteSpace();
            if (context.NotEnd())
            {
                throw ParseErrors.InvalidText(context);
            }
            return limitInfo;
        }
    }

}
