using System;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace YS.Knife.Query.Parser
{
    public class ParseContext
    {
        public static readonly Func<char, bool> IsValidNameFirstChar = ch => char.IsLetter(ch) || ch == '_';
        public static readonly Func<char, bool> IsValidNameChar = ch => char.IsLetterOrDigit(ch) || ch == '_';

        public CultureInfo CurrentCulture { get; }

        public char NumberDecimal { get; } // 小数点
        public char NumberNegativeSign { get; }// 负号
        public char NumberPositiveSign { get; } // 正号
        public char NumberGroupSeparator { get; }// 分组符号


        public ParseContext(string text, CultureInfo cultureInfo)
        {
            this.Text = text;
            this.TotalLength = text.Length;
            this.CurrentCulture = cultureInfo;
            this.NumberDecimal = cultureInfo.NumberFormat.NumberDecimalSeparator[0];
            this.NumberNegativeSign = cultureInfo.NumberFormat.NegativeSign[0];
            this.NumberPositiveSign = cultureInfo.NumberFormat.PositiveSign[0];
            // this._numberGroupSeparator = cultureInfo.NumberFormat.NumberGroupSeparator[0];
            // default number group separator will conflict with array separator
            // eg. [1,234], so use '_' instead of default number group separator
            this.NumberGroupSeparator = '_';
        }
        public char Current()
        {
            if (Index >= TotalLength)
            {
                throw ParseErrors.InvalidText(this);
            }
            return Text[Index];
        }
        public bool NotEnd()
        {
            return Index < TotalLength;
        }
        public bool End()
        {
            return Index >= TotalLength;
        }

        public string Text;
        public int TotalLength;
        public int Index;


        public (bool, string) TryParseName()
        {
            if (NotEnd() && IsValidNameFirstChar(Current()))
            {
                int startIndex = this.Index;
                Index++;
                while (NotEnd() && IsValidNameChar(Current()))
                {
                    Index++;
                }
                return (true, Text.Substring(startIndex, Index - startIndex));
            }
            else
            {
                return (false, null);
            }
        }
        public (bool, int) TryParseUnsignInt32()
        {
            int startIndex = Index;
            while (NotEnd() && char.IsDigit(Current()))
            {
                Index++;
            }
            if (Index > startIndex)
            {
                string numText = Text.Substring(startIndex, Index - startIndex);
                try
                {
                    return (true, int.Parse(numText));
                }
                catch (Exception ex)
                {
                    throw ParseErrors.InvalidNumberValue(this, numText, ex);
                }
            }
            else
            {
                return (false, 0);
            }

        }
    }
}
