using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YS.Knife.Query.Parser
{
    class ParseErrors
    {
        public static Exception InvalidText(ParseContext context)
        {
            throw new Exception($"Invalid text near index {context.Index}.");
        }
        public static Exception InvalidFieldNameText(ParseContext context)
        {
            throw new Exception($"Invalid field name near index {context.Index}.");
        }
        public static Exception InvalidFilterType(ParseContext context)
        {
            throw new Exception($"Invalid filter type near index {context.Index}.");
        }
        public static Exception InvalidFilterType(ParseContext context, string code)
        {
            throw new Exception($"Invalid filter type '{code}' near index {context.Index}.");
        }
        public static Exception InvalidValue(ParseContext context)
        {
            throw new Exception($"Invalid value near index {context.Index}.");
        }
        public static Exception InvalidKeywordValue(ParseContext context, string keyword)
        {
            throw new Exception($"Invalid keyword '{keyword}' near index {context.Index}.");
        }
        public static Exception InvalidStringValue(ParseContext context, string str, Exception inner)
        {
            throw new Exception($"Invalid string '{str}' near index {context.Index}.", inner);
        }
        public static Exception InvalidNumberValue(ParseContext context, string str, Exception inner)
        {
            throw new Exception($"Invalid number '{str}' near index {context.Index}.", inner);
        }

        public static Exception MissOpenBracket(ParseContext context)
        {
            throw new Exception($"Invalid expression, missing open bracket near index {context.Index}.");
        }
        public static Exception MissCloseBracket(ParseContext context)
        {
            throw new Exception($"Invalid expression, missing close bracket near index {context.Index}.");
        }
        public static Exception ExpectedCharNotFound(ParseContext context, char ch)
        {
            throw new Exception($"Invalid expression near index {context.Index}, expect char '{ch}' not found.");
        }
        public static Exception ParaseLimitNumberError(ParseContext context)
        {
            throw new Exception($"Invalid expression near index {context.Index}, parse limit error.");
        }

        public static Exception FunctionArgumentLessThan(ParseContext context, string functionName, int minArgumentLength)
        {
            throw new Exception($"The number of arguments of the function '{functionName}' less than min required {minArgumentLength}.");
        }
        public static Exception FunctionArgumentGreatThan(ParseContext context, string functionName, int maxArgumentLength)
        {
            throw new Exception($"The number of arguments of the function '{functionName}' exceeds the limit {maxArgumentLength}.");
        }

        #region select
        public static Exception OnlySupportCollectionFunctionInCurlyBracket(ParseContext context)
        {
            throw new Exception($"Invalid select expression near index {context.Index}, only support 'where', 'orderby', 'limit' function in curly brackets.");
        }
        public static Exception DuplicateCollectionFunctionInCurlyBracket(ParseContext context, string functionName)
        {
            throw new Exception($"Invalid select expression near index {context.Index}, duplicate function '{functionName}'.");
        }
        #endregion

    }
}
