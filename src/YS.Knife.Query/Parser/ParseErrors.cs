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
            throw new ParseException($"Invalid text near index {context.Index}.");
        }
        public static Exception InvalidFieldNameText(ParseContext context)
        {
            throw new ParseException($"Invalid field name near index {context.Index}.");
        }
        public static Exception InvalidFilterType(ParseContext context)
        {
            throw new ParseException($"Invalid filter type near index {context.Index}.");
        }
        public static Exception InvalidFilterType(ParseContext context, string code)
        {
            throw new ParseException($"Invalid filter type '{code}' near index {context.Index}.");
        }
        public static Exception InvalidValue(ParseContext context)
        {
            throw new ParseException($"Invalid value near index {context.Index}.");
        }
        public static Exception InvalidKeywordValue(ParseContext context, string keyword)
        {
            throw new ParseException($"Invalid keyword '{keyword}' near index {context.Index}.");
        }
        public static Exception InvalidStringValue(ParseContext context, string str, Exception inner)
        {
            throw new ParseException($"Invalid string '{str}' near index {context.Index}.", inner);
        }
        public static Exception InvalidNumberValue(ParseContext context, string str, Exception inner)
        {
            throw new ParseException($"Invalid number '{str}' near index {context.Index}.", inner);
        }

        public static Exception MissOpenBracket(ParseContext context)
        {
            throw new ParseException($"Invalid expression, missing open bracket near index {context.Index}.");
        }
        public static Exception MissCloseBracket(ParseContext context)
        {
            throw new ParseException($"Invalid expression, missing close bracket near index {context.Index}.");
        }
        public static Exception ExpectedCharNotFound(ParseContext context, char ch)
        {
            throw new ParseException($"Invalid expression near index {context.Index}, expect char '{ch}' not found.");
        }
        public static Exception ParaseLimitNumberError(ParseContext context)
        {
            throw new ParseException($"Invalid expression near index {context.Index}, parse limit error.");
        }

        #region select
        public static Exception OnlySupportCollectionFunctionInCurlyBracket(ParseContext context)
        {
            throw new ParseException($"Invalid select expression near index {context.Index}, only support 'where', 'orderby', 'limit' function in curly brackets.");
        }
        public static Exception CollectionLimitFunctionMissingBody(ParseContext context)
        {
            throw new ParseException($"Invalid select expression near index {context.Index}, the function limit missing body");
        }
        public static Exception CollectionOrderByFunctionMissingBody(ParseContext context)
        {
            throw new ParseException($"Invalid select expression near index {context.Index}, the function orderby missing body");
        }
        public static Exception CollectionWhereFunctionMissingBody(ParseContext context)
        {
            throw new ParseException($"Invalid select expression near index {context.Index}, the function where missing body");
        }
        public static Exception DuplicateCollectionFunctionInCurlyBracket(ParseContext context, string functionName)
        {
            throw new ParseException($"Invalid select expression near index {context.Index}, duplicate function '{functionName}'.");
        }
        #endregion

        public static Exception FunctionParametersExceedsLimit(ParseContext context)
        {
            throw new ParseException($"Invalid function body near index {context.Index}, the number of function parameters exceeds the range");
        }

    }
}
