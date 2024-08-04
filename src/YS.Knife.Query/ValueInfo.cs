using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using YS.Knife.Query.Parser;

namespace YS.Knife.Query
{
    // [DebuggerDisplay("{ToString()}")]
    public class ValueInfo
    {
        public bool IsConstant
        {
            get
            {
                return this.NavigatePaths?.Count == 1 && this.NavigatePaths[0].IsConstant;
            }
        }
        public List<ValuePath> NavigatePaths { get; set; }
        public override string ToString()
        {
            if (NavigatePaths?.Count > 0)
            {
                var names = (NavigatePaths ?? Enumerable.Empty<ValuePath>()).Where(p => p != null).Select(p => p.ToString());
                return string.Join(".", names);
            }
            else
            {
                return ValueToString(null);

            }

            string ValueToString(object value, bool convertCollection = true)
            {
                if (value == null || value == DBNull.Value)
                {
                    return "null";
                }
                else if (value is string str)
                {
                    return Repr(str);
                }
                else if (value is bool)
                {
                    return value.ToString().ToLowerInvariant();
                }
                else if (value is int || value is short || value is long || value is float || value is double ||
                         value is decimal
                         || value is uint || value is ushort || value is ulong || value is sbyte || value is byte)
                {
                    return value.ToString();
                }
                else if (convertCollection && value is IEnumerable items)
                {
                    var body = string.Join(',', items.OfType<object>().Select(p => ValueToString(p, false)));
                    return string.Format($"[{body}]");
                }
                else
                {
                    return Repr(value.ToString());
                }
            }
            string Repr(string str)
            {
                return $"\"{Regex.Escape(str).Replace("\"", "\\\"")}\"";
            }
        }

        public static ValueInfo Parse(string valueExpression) => Parse(valueExpression, CultureInfo.CurrentCulture);

        public static ValueInfo Parse(string valueExpression, CultureInfo cultureInfo)
        {
            return new QueryExpressionParser(cultureInfo).ParseValue(valueExpression);
        }

        public static ValueInfo FromConstantValue(object value)
        {
            return new ValueInfo
            {
                NavigatePaths = new List<ValuePath>
                {
                    new ValuePath{ IsConstant=true, ConstantValue=value}
                }
            };
        }
        public static ValueInfo FromPaths(List<ValuePath> navigatePaths)
        {
            return new ValueInfo
            {
                NavigatePaths = navigatePaths ?? new List<ValuePath>()
            };
        }
    }

}
