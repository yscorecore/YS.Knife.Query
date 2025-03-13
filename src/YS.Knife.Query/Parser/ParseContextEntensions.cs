using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using YS.Knife.Query.Functions.Specials;

namespace YS.Knife.Query.Parser
{
    public static class ParseContextEntensions
    {
        internal static readonly HashSet<string> filterArgumentFunction = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase)
        {
            nameof(Where)
        };
        internal static readonly HashSet<string> orderByArgumentFunction = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase)
        {
            nameof(OrderBy)
        };
        internal static readonly HashSet<string> limitArgumentFunction = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase)
        {
            nameof(Limit)
        };

        internal static readonly Dictionary<string, object> KeyWordValues = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase)
        {
            ["true"] = true,
            ["false"] = false,
            ["null"] = null
        };
        internal static readonly Dictionary<string, CombinSymbol> OpTypeCodes = new Dictionary<string, CombinSymbol>(StringComparer.InvariantCultureIgnoreCase)
        {
            [FilterInfo.Operator_And] = CombinSymbol.AndItems,
            [FilterInfo.Operator_Or] = CombinSymbol.OrItems
        };

        internal static readonly Dictionary<string, Operator> FilterTypeCodes =
          typeof(Operator).GetFields(BindingFlags.Public | BindingFlags.Static)
            .SelectMany(p => p.GetCustomAttributes<OperatorCodeAttribute>().Select(c => (c.Code, (Operator)p.GetValue(null))))
            .ToDictionary(p => p.Code, p => p.Item2, StringComparer.InvariantCultureIgnoreCase);

        private static readonly Func<char, bool> IsValidNameFirstChar = ch => char.IsLetter(ch) || ch == '_';
        private static readonly Func<char, bool> IsValidNameChar = ch => char.IsLetterOrDigit(ch) || ch == '_';
        private static readonly Func<char, bool> IsWhiteSpace = ch => ch == ' ' || ch == '\t';
        private static readonly Func<char, bool> IsEscapeChar = ch => ch == '\\';
        private static readonly Func<char, bool> IsOperationChar = ch => ch == '=' || ch == '<' || ch == '>' || ch == '!';
        private static readonly Func<char, bool> IsStringWrapChar = ch => ch == '\'' || ch == '"';
        private static bool IsNumberStartChar(char current, ParseContext context) => char.IsDigit(current) || current == context.NumberDecimal || current == context.NumberNegativeSign || current == context.NumberPositiveSign;


        public static bool SkipWhiteSpace(this ParseContext context)
        {
            while (context.NotEnd())
            {
                if (IsWhiteSpace(context.Text[context.Index]))
                {
                    context.Index++;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }
        public static void SkipWhiteSpaceAndFirstChar(this ParseContext context, char ch)
        {
            if (context.SkipWhiteSpace())
            {
                if (context.Current() != ch)
                {
                    throw ParseErrors.ExpectedCharNotFound(context, ch);
                }
                else
                {
                    context.Index++;
                }
            }
            else
            {
                throw ParseErrors.ExpectedCharNotFound(context, ch);
            }
        }

        public static ValueInfo ParseValueInfo(this ParseContext context)
        {
            context.SkipWhiteSpace();

            var propertyPaths = ParsePropertyPaths(context);

            return ValueInfo.FromPaths(propertyPaths);
        }


        public static string ParseName(this ParseContext context)
        {
            context.SkipWhiteSpace();
            int startIndex = context.Index;
            if (!IsValidNameFirstChar(context.Current()))
            {
                throw ParseErrors.InvalidFieldNameText(context);
            }
            context.Index++;// first char

            while (context.NotEnd() && IsValidNameChar(context.Current()))
            {
                context.Index++;
            }

            return context.Text.Substring(startIndex, context.Index - startIndex);
        }
        public static (bool, object) TryParseValue(this ParseContext context)
        {
            var originIndex = context.Index;
            context.SkipWhiteSpace();
            if (context.End())
            {
                context.Index = originIndex;
                return (false, null);
            }
            var current = context.Current();
            if (IsStringWrapChar(current))
            {
                //string
                return (true, ParseStringValue(context, current));
            }
            else if (char.IsLetter(current))
            {
                //keyword eg
                string name = ParseName(context);
                if (KeyWordValues.TryGetValue(name, out var val))
                {
                    return (true, val);
                }
                else
                {
                    context.Index = originIndex;
                    return (false, null);
                }
            }
            else if (IsNumberStartChar(current, context))
            {
                //number
                return (true, ParseNumberValue(context));
            }
            else if (current == '[')
            {
                return (true, ParseArrayValue(context));
                //array
            }
            return (false, null);


            string ParseStringValue(ParseContext context, char wrapChar = '"')
            {
                // skip start
                context.Index++;
                bool lastIsEscapeChar = false;
                var startIndex = context.Index;

                while (context.NotEnd())
                {
                    if (lastIsEscapeChar)
                    {
                        lastIsEscapeChar = false;
                        context.Index++;
                        continue;
                    }
                    else
                    {
                        var current = context.Current();
                        if (current == wrapChar)
                        {
                            break;
                        }
                        lastIsEscapeChar = IsEscapeChar(current);
                        context.Index++;
                    }

                }
                string origin = context.Text.Substring(startIndex, context.Index - startIndex);
                // skip end
                context.Index++;
                try
                {
                    return Regex.Unescape(origin);
                }
                catch (Exception ex)
                {
                    throw ParseErrors.InvalidStringValue(context, origin, ex);
                }
            }

            object ParseNumberValue(ParseContext context)
            {
                int startIndex = context.Index;
                bool hasDecimsl = context.Current() == context.NumberDecimal;
                bool hasDigit = char.IsDigit(context.Current());
                // skip first char
                context.Index++;
                while (context.NotEnd())
                {
                    var current = context.Current();
                    if (char.IsDigit(current))
                    {
                        hasDigit = true;
                        context.Index++;
                    }
                    else if (current == context.NumberGroupSeparator)
                    {
                        if (hasDecimsl)
                        {
                            throw ParseErrors.InvalidValue(context);
                        }
                        context.Index++;
                    }
                    else if (current == context.NumberDecimal)
                    {
                        if (hasDecimsl)
                        {
                            throw ParseErrors.InvalidValue(context);
                        }
                        else
                        {
                            hasDecimsl = true;
                        }
                        context.Index++;
                    }
                    else
                    {
                        break;
                    }
                }
                if (!hasDigit)
                {
                    throw ParseErrors.InvalidValue(context);
                }
                var numString = context.Text.Substring(startIndex, context.Index - startIndex);
                try
                {
                    if (hasDecimsl)
                    {
                        return double.Parse(numString.Replace(context.NumberGroupSeparator.ToString(), string.Empty), NumberStyles.Any, context.CurrentCulture);

                    }
                    else
                    {
                        return int.Parse(numString.Replace(context.NumberGroupSeparator.ToString(), string.Empty), NumberStyles.Any, context.CurrentCulture);

                    }

                }
                catch (Exception ex)
                {
                    throw ParseErrors.InvalidNumberValue(context, numString, ex);
                }
            }

            object ParseArrayValue(ParseContext context)
            {
                // skip start [
                List<object> datas = new List<object>();
                context.Index++;
                while (context.NotEnd())
                {
                    context.SkipWhiteSpace();
                    var current = context.Current();
                    if (current == ']')
                    {
                        context.Index++;
                        break;
                    }
                    datas.Add(ParseValue(context, false));
                    context.SkipWhiteSpace();
                    if (context.Current() == ',')
                    {
                        context.Index++;
                    }

                }
                return ConvertToSameTypeArray(datas);

            }

            object ConvertToSameTypeArray(List<object> datas)
            {

                if (datas.Count == 0)
                {
                    return Array.Empty<string>();
                }
                else
                {
                    var item = datas.Where(p => p != null).FirstOrDefault();
                    if (item == null)
                    {
                        return datas.Select(p => (string)p).ToArray();
                    }
                    else
                    {
                        var itemType = item.GetType();
                        var isValueType = itemType.IsValueType;
                        bool hasNull = false;
                        for (int i = 0; i < datas.Count; i++)
                        {
                            if (datas[i] == null)
                            {
                                hasNull = true;
                            }
                            else
                            {
                                datas[i] = Convert.ChangeType(datas[i], item.GetType());
                            }
                        }
                        var res = (hasNull && itemType.IsValueType) ?
                            Array.CreateInstance(typeof(Nullable<>).MakeGenericType(itemType), datas.Count)
                            : Array.CreateInstance(itemType, datas.Count);
                        for (var i = 0; i < datas.Count; i++)
                        {
                            res.SetValue(datas[i], i);
                        }
                        return res;
                    }
                }

            }
            object ParseKeywordValue(ParseContext context)
            {
                int startIndex = context.Index;
                while (context.NotEnd() && IsValidNameChar(context.Current()))
                {
                    context.Index++;
                }
                string keyWord = context.Text.Substring(startIndex, context.Index - startIndex);
                if (KeyWordValues.TryGetValue(keyWord, out object value))
                {
                    return value;
                }
                else
                {
                    throw ParseErrors.InvalidKeywordValue(context, keyWord);
                }
            }

            object ParseValue(ParseContext context, bool parseArray = true)
            {
                context.SkipWhiteSpace();
                var current = context.Current();
                if (IsStringWrapChar(current))
                {
                    //string
                    return ParseStringValue(context, current);
                }
                else if (char.IsLetter(current))
                {
                    //keyword
                    return ParseKeywordValue(context);
                }
                else if (IsNumberStartChar(current, context))
                {
                    //number
                    return ParseNumberValue(context);
                }
                else if (parseArray && current == '[')
                {
                    return ParseArrayValue(context);
                    //array
                }
                else
                {
                    throw ParseErrors.InvalidValue(context);
                }
            }
        }

        public static List<ValuePath> ParsePropertyPaths(this ParseContext context)
        {
            List<ValuePath> names = new List<ValuePath>();
            while (context.NotEnd())
            {
                //只允许第一个是常数，e.g "abc".toupper()

                if (names.Count == 0)
                {
                    var (isValue, value) = TryParseValue(context);
                    if (isValue)
                    {
                        names.Add(new ValuePath { ConstantValue = value, IsConstant = true });
                        context.SkipWhiteSpace();
                        if (context.End())
                        {
                            break;
                        }
                        else if (context.Current() == '.')
                        {
                            context.Index++;
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    }
                }


                var name = ParseName(context);
                context.SkipWhiteSpace();
                if (context.End())
                {
                    names.Add(new ValuePath { Name = name });
                    break;
                }
                else if (context.Current() == '.')
                {
                    // a.b
                    names.Add(new ValuePath { Name = name });
                    context.Index++;

                }

                else if (context.Current() == '(')
                {
                    var args = context.ParseFunctionArguments(name);
                    var nameInfo = new ValuePath { Name = name, IsFunction = true, FunctionArgs = args };
                    context.SkipWhiteSpace();
                    names.Add(nameInfo);
                    if (context.NotEnd())
                    {
                        if (context.Current() == '.')
                        {
                            context.Index++;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else
                {
                    names.Add(new ValuePath { Name = name });
                    break;
                }




            }
            return names;


        }
        private static ValueInfo[] ParseFunctionArguments(this ParseContext context, string name)
        {
            // skip open 
            List<ValueInfo> args = new List<ValueInfo>();
            context.SkipWhiteSpaceAndFirstChar('(');

            while (context.NotEnd())
            {
                if (!context.SkipWhiteSpace())
                {
                    break;
                }
                if (context.Current() == ')')
                {
                    break;
                }
                var (arg, closed) = context.ParseFunctionArgument(name);
                args.Add(arg);

                if (context.SkipWhiteSpace())
                {
                    if (closed && context.Current() != ')')
                    {
                        throw ParseErrors.FunctionParametersExceedsLimit(context);
                    }
                    if (context.Current() == ',')
                    {
                        context.Index++;
                    }
                    else if (context.Current() == ')')
                    {
                        break;
                    }
                    else
                    {
                        throw ParseErrors.InvalidText(context);
                    }
                }
            }
            context.SkipWhiteSpaceAndFirstChar(')');
            return args.ToArray();
        }
        private static (ValueInfo, bool Closed) ParseFunctionArgument(this ParseContext context, string name)
        {
            if (filterArgumentFunction.Contains(name))
            {
                var filter = context.ParseFilterInfo();
                return (ValueInfo.FromConstantValue(filter), true);
            }
            else if (orderByArgumentFunction.Contains(name))
            {
                var orderBy = context.ParseOrderByInfo();
                return (ValueInfo.FromConstantValue(orderBy), true);
            }
            else if (limitArgumentFunction.Contains(name))
            {
                var limitInfo = context.ParseLimitInfo();
                return (ValueInfo.FromConstantValue(limitInfo), true);
            }
            else
            {
                return (context.ParseValueInfo(), false);

            }
        }

        public static FilterInfo ParseFilterInfo(this ParseContext context)
        {
            context.SkipWhiteSpace();
            if (context.Current() == '(')
            {
                return ParseCombinFilter(context);
            }
            else
            {
                return ParseSingleItemOne(context);
            }
            FilterInfo ParseCombinFilter(ParseContext context)
            {
                List<FilterInfo> orItems = new List<FilterInfo>();
                CombinSymbol lastOpType = CombinSymbol.OrItems;
                while (context.NotEnd())
                {
                    // skip start bracket
                    context.SkipWhiteSpace();
                    if (context.Current() != '(')
                    {
                        throw ParseErrors.MissOpenBracket(context);
                    }
                    context.Index++;
                    context.SkipWhiteSpace();
                    var inner = ParseFilterInfo(context);
                    context.SkipWhiteSpace();
                    if (context.End() || context.Current() != ')')
                    {
                        throw ParseErrors.MissCloseBracket(context);
                    }
                    context.Index++;

                    if (lastOpType == CombinSymbol.OrItems || orItems.Count == 0)
                    {
                        orItems.Add(inner);
                    }
                    else
                    {
                        orItems[^1] = orItems[^1].AndAlso(inner);
                    }

                    CombinSymbol? opType = TryParseOpType(context);

                    if (opType == null)
                    {
                        break;
                    }
                    else
                    {
                        lastOpType = opType.Value;
                    }
                }
                return orItems.Count > 1 ? new FilterInfo { OpType = CombinSymbol.OrItems, Items = orItems } : orItems.FirstOrDefault();
            }
            CombinSymbol? TryParseOpType(ParseContext context)
            {
                var originIndex = context.Index;

                context.SkipWhiteSpace();
                var wordStartIndex = context.Index;
                while (context.NotEnd() && IsValidNameChar(context.Current()))
                {
                    context.Index++;
                }
                var word = context.Text.Substring(wordStartIndex, context.Index - wordStartIndex);
                if (OpTypeCodes.TryGetValue(word, out var opType))
                {
                    return opType;
                }
                else
                {
                    // reset index
                    context.Index = originIndex;
                    return null;
                }
            }

            FilterInfo ParseSingleItemOne(ParseContext context)
            {
                var leftValue = context.ParseValueInfo();

                var type = ParseType(context);

                var rightValue = context.ParseValueInfo();

                return new FilterInfo()
                {
                    OpType = CombinSymbol.SingleItem,
                    Left = leftValue,
                    Operator = type,
                    Right = rightValue,
                };

            }

            Operator ParseType(ParseContext context)
            {
                context.SkipWhiteSpace();
                int startIndex = context.Index;
                if (char.IsLetter(context.Current()))
                {
                    while (context.NotEnd() && IsValidNameChar(context.Current()))
                    {
                        context.Index++;
                    }
                    string opCode = context.Text.Substring(startIndex, context.Index - startIndex);
                    if (FilterTypeCodes.TryGetValue(opCode.ToLowerInvariant(), out Operator filterType))
                    {
                        return filterType;
                    }
                    else
                    {
                        throw ParseErrors.InvalidFilterType(context, opCode);
                    }
                }
                else if (IsOperationChar(context.Current()))
                {
                    while (context.NotEnd() && IsOperationChar(context.Current()))
                    {
                        context.Index++;
                    }
                    string opCode = context.Text.Substring(startIndex, context.Index - startIndex);
                    if (FilterTypeCodes.TryGetValue(opCode, out Operator filterType))
                    {
                        return filterType;
                    }
                    else
                    {
                        throw ParseErrors.InvalidFilterType(context, opCode);
                    }
                }
                else
                {
                    throw ParseErrors.InvalidFilterType(context);
                }
            }
        }

        public static OrderByInfo ParseOrderByInfo(this ParseContext context)
        {
            OrderByInfo orderInfo = new OrderByInfo();
            while (context.SkipWhiteSpace())
            {
                var startIndex = context.Index;
                var paths = context.ParsePropertyPaths();
                try
                {
                    orderInfo.Add(CreateOrderByItemFromValuePaths(paths));
                }
                catch (Exception ex)
                {
                    throw ParseErrors.InvalidOrderbyItem(context, startIndex, ex);
                }
                if (context.SkipWhiteSpace() && context.Current() == ',')
                {
                    context.Index++;
                }
                else
                {
                    break;
                }
            }
            return orderInfo.HasItems() ? orderInfo : null;
        }
        internal static OrderByItem CreateOrderByItemFromValuePaths(List<ValuePath> paths)
        {
            var properties = new List<ValuePath>();
            object orderByType = null;
            for (var i = 0; i < paths.Count; i++)
            {
                var path = paths[i];
                if (path.IsFunction)
                {
                    if (orderByType == null)
                    {
                        if (Enum.TryParse(typeof(OrderByType), path.Name, true, out orderByType))
                        {
                            if (path.FunctionArgs?.Length > 0)
                            {
                                throw new ParseException("orderby function should has no argument.");
                            }
                        }
                        else
                        {
                            properties.Add(path);
                        }
                    }
                    else
                    {
                        throw new ParseException($"invalid function '{path.Name}' after '{orderByType}' function");
                    }
                }
                else if (path.IsConstant)
                {
                    throw new ParseException("orderby item should not be constant.");
                }
                else
                {
                    if (orderByType != null)
                    {
                        throw new ParseException($"invalid property '{path.Name}' after '{orderByType}' function");
                    }
                    else
                    {
                        properties.Add(path);
                    }

                }
            }

            return new OrderByItem
            {
                NavigatePaths = properties,
                OrderByType = orderByType == null ? OrderByType.Asc : (OrderByType)orderByType
            };
        }
        public static AggInfo ParseAggInfo(this ParseContext context)
        {
            AggInfo aggInfo = new AggInfo();
            while (context.SkipWhiteSpace())
            {
                var startIndex = context.Index;
                var paths = context.ParsePropertyPaths();
                try
                {
                    aggInfo.Add(CreateAggItemFromValuePaths(paths));
                }
                catch (Exception ex)
                {
                    throw ParseErrors.InvalidAggItem(context, startIndex, ex);
                }
                if (context.SkipWhiteSpace() && context.Current() == ',')
                {
                    context.Index++;
                }
                else
                {
                    break;
                }
            }
            return aggInfo.HasItems() ? aggInfo : null;
        }
        internal static AggItem CreateAggItemFromValuePaths(List<ValuePath> paths)
        {
            var properties = new List<ValuePath>();
            object aggType = null;
            var alias = default(string);
            for (var i = 0; i < paths.Count; i++)
            {
                var path = paths[i];
                if (path.IsFunction)
                {
                    if (properties.Count == 0)
                    {
                        throw new ParseException("missing agg property");
                    }
                    else if (aggType == null)
                    {
                        if (Enum.TryParse(typeof(AggType), path.Name, true, out aggType))
                        {
                            if (path.FunctionArgs?.Length > 0)
                            {
                                throw new ParseException("agg function should has no argument.");
                            }
                        }
                        else
                        {
                            throw new ParseException($"unknow agg function type '{path.Name}'");
                        }
                    }
                    else if (alias == null)
                    {
                        //as
                        if (path.Name.Equals("as", StringComparison.InvariantCultureIgnoreCase))
                        {
                            if (path.FunctionArgs.Length == 1)
                            {
                                var firstArg = path.FunctionArgs[0];
                                if (firstArg.IsConstant)
                                {
                                    alias = Convert.ToString(firstArg.ConstantValue);
                                }
                                else
                                {
                                    alias = firstArg.ToString();
                                }
                            }
                            else
                            {
                                throw new ParseException("function 'as' only has one argument.");
                            }
                        }
                        else
                        {
                            throw new ParseException($"invalid function {path.Name} after 'as'.");
                        }
                    }
                    else
                    {
                        throw new ParseException($"invalid function {path.Name} after '{alias}'.");
                    }
                }
                else if (path.IsConstant)
                {
                    throw new ParseException("agg item should not be constant.");
                }
                else
                {
                    properties.Add(path);
                }
            }

            return new AggItem
            {
                NavigatePaths = properties,
                AggType = aggType == null ? AggType.Sum : (AggType)aggType,
                AggName = alias
            };
        }
        public static LimitInfo ParseLimitInfo(this ParseContext context)
        {
            if (context.SkipWhiteSpace())
            {
                var (first, firstNumber) = TryParseUnsignInt32(context);
                if (!first)
                {
                    throw ParseErrors.ParaseLimitNumberError(context);
                }
                if (context.SkipWhiteSpace() && context.Current() == ',')
                {
                    context.Index++;
                    var (second, secondNumber) = TryParseUnsignInt32(context);
                    if (second)
                    {
                        return new LimitInfo { Offset = firstNumber, Limit = secondNumber };
                    }
                    else
                    {
                        return new LimitInfo { Limit = firstNumber };
                    }
                }
                else
                {
                    return new LimitInfo { Limit = firstNumber };
                }
            }
            else
            {
                throw ParseErrors.ParaseLimitNumberError(context);
            }


            (bool, int) TryParseUnsignInt32(ParseContext context)
            {
                int startIndex = context.Index;
                context.SkipWhiteSpace();
                while (context.NotEnd() && char.IsDigit(context.Current()))
                {
                    context.Index++;
                }
                if (context.Index > startIndex)
                {
                    string numText = context.Text.Substring(startIndex, context.Index - startIndex);
                    try
                    {
                        return (true, int.Parse(numText));
                    }
                    catch (Exception ex)
                    {
                        throw ParseErrors.InvalidNumberValue(context, numText, ex);
                    }
                }
                else
                {
                    return (false, 0);
                }

            }
        }

        public static SelectInfo ParseSelectInfo(this ParseContext context)
        {
            SelectInfo selectInfo = new SelectInfo
            {
                Items = new List<SelectItem>()
            };
            var exclude = false;
            while (context.SkipWhiteSpace())
            {
                char current = context.Current();
                exclude = false;
                if (current == '+' || current == '-')
                {
                    exclude = current == '-';
                    context.Index++;
                    context.SkipWhiteSpace();
                    if (context.End())
                    {
                        throw ParseErrors.InvalidText(context);
                    }
                }

                var (found, name) = context.TryParseNameOrSpecialName();
                if (found)
                {
                    selectInfo.Items.Add(ParseSelectItem(exclude, name, context));
                    if (context.SkipWhiteSpace() && context.Current() == ',')
                    {
                        context.Index++;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            return selectInfo;
            SelectItem ParseSelectItem(bool exclude, string name, ParseContext context)
            {
                SelectItem item = new SelectItem { Name = name, Exclude = exclude };
                if (context.SkipWhiteSpace() && context.Current() == '{')
                {
                    // parse collection infos
                    ParseCollectionInfos2(item, context);
                }
                if (context.SkipWhiteSpace() && context.Current() == '(')
                {
                    // parse sub items
                    context.Index++;
                    item.SubItems = ParseSelectInfo(context).Items;
                    if (context.SkipWhiteSpace() == false || context.Current() != ')')
                    {
                        throw ParseErrors.MissCloseBracket(context);
                    }
                    else
                    {
                        context.Index++;
                    }
                }
                return item;
            }
            void ParseCollectionInfos2(SelectItem selectItem, ParseContext context)
            {
                context.SkipWhiteSpaceAndFirstChar('{');
                var set = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase);
                do
                {
                    if (!context.SkipWhiteSpace())
                    {
                        break;
                    }
                    if (context.Current() == '}')
                    {
                        break;
                    }
                    var valueInfo = context.ParseValueInfo();

                    if (valueInfo.IsConstant || valueInfo.NavigatePaths.Count != 1 || valueInfo.NavigatePaths[0].IsFunction == false)
                    {
                        throw ParseErrors.OnlySupportCollectionFunctionInCurlyBracket(context);
                    }
                    var functionName = valueInfo.NavigatePaths[0].Name;
                    if (set.Contains(functionName))
                    {
                        throw ParseErrors.DuplicateCollectionFunctionInCurlyBracket(context, functionName);
                    }
                    else
                    {
                        set.Add(functionName);
                    }
                    SetCollectionFilterValue(context, selectItem, valueInfo.NavigatePaths[0]);
                    if (!context.SkipWhiteSpace())
                    {
                        break;
                    }
                    if (context.Current() == ',')
                    {
                        context.Index++;
                    }
                    else if (context.Current() == '}')
                    {
                        break;
                    }
                    else
                    {
                        throw ParseErrors.InvalidText(context);
                    }
                }
                while (true);
                context.SkipWhiteSpaceAndFirstChar('}');

            }
            void SetCollectionFilterValue(ParseContext context, SelectItem selectItem, ValuePath valuePath)
            {
                var firstArgument = valuePath.FunctionArgs.FirstOrDefault()?.ConstantValue;
                if (nameof(Where).Equals(valuePath.Name, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (firstArgument is FilterInfo filterInfo)
                    {
                        selectItem.CollectionFilter = filterInfo;
                    }
                    else
                    {
                        throw ParseErrors.CollectionWhereFunctionMissingBody(context);
                    }

                }
                else if (nameof(OrderBy).Equals(valuePath.Name, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (firstArgument is OrderByInfo orderByInfo)
                    {
                        selectItem.CollectionOrderBy = orderByInfo;
                    }
                    else
                    {
                        throw ParseErrors.CollectionOrderByFunctionMissingBody(context);

                    }

                }
                else if (nameof(Limit).Equals(valuePath.Name, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (firstArgument is LimitInfo limitInfo)
                    {
                        selectItem.CollectionLimit = limitInfo;
                    }
                    else
                    {
                        throw ParseErrors.CollectionLimitFunctionMissingBody(context);

                    }

                }
                else
                {
                    throw ParseErrors.OnlySupportCollectionFunctionInCurlyBracket(context);
                }

            }
        }

        private static (bool, string) TryParseNameOrSpecialName(this ParseContext context)
        {
            var startIndex = context.Index;
            char current = context.Current();
            if (current == '[')
            {
                context.Index++;
                context.SkipWhiteSpace();
                string name = context.ParseName();
                context.SkipWhiteSpace();
                if (context.Current() == ']')
                {
                    context.Index++;
                    return (true, $"[{name}]");
                }
                else
                {
                    throw ParseErrors.MissCloseSquarebrackets(context);
                }
            }
            else
            {
                return context.TryParseName();
            }

        }
    }
}
