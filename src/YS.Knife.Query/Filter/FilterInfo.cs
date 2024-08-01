using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace YS.Knife.Query
{
    [Serializable]
    public class FilterInfo
    {
        internal const string Operator_And = "and";
        internal const string Operator_Or = "or";
        private static readonly Dictionary<Operator, string> OperatorTypeCodeDictionary =
            typeof(Operator).GetFields(BindingFlags.Public | BindingFlags.Static).ToDictionary(
                p => (Operator)p.GetValue(null),
                p => p.GetCustomAttributes<OperatorCodeAttribute>().Select(p => p.Code).First());

        public ValueInfo Left { get; set; }
        public ValueInfo Right { get; set; }
        public Operator Operator { get; set; }
        public CombinSymbol OpType { get; set; }
        public List<FilterInfo> Items { get; set; }

        public FilterInfo AndAlso(FilterInfo other)
        {
            if (other == null)
            {
                return this;
            }

            if (OpType == CombinSymbol.AndItems)
            {
                if (other.OpType == CombinSymbol.AndItems)
                {
                    Items.AddRange(other.Items ?? Enumerable.Empty<FilterInfo>());
                    return this;
                }
                else
                {
                    Items.Add(other);
                    return this;
                }
            }
            else
            {
                return new FilterInfo() { OpType = CombinSymbol.AndItems, Items = new List<FilterInfo>() { this, other } };
            }
        }
        public FilterInfo AndAlso(string fieldPaths, Operator filterOperator, object value)
        {
            return this.AndAlso(FilterInfo.CreateItem(fieldPaths, filterOperator, value));
        }
        public FilterInfo OrElse(FilterInfo other)
        {
            if (other == null)
            {
                return this;
            }

            if (OpType == CombinSymbol.OrItems)
            {
                if (other.OpType == CombinSymbol.OrItems)
                {
                    Items.AddRange(other.Items);
                }
                else
                {
                    Items.Add(other);
                }

                return this;
            }
            else
            {
                return new FilterInfo() { OpType = CombinSymbol.OrItems, Items = new List<FilterInfo>() { this, other } };
            }
        }

        public FilterInfo OrElse(string fieldPaths, Operator filterOperator, object value)
        {
            return this.OrElse(FilterInfo.CreateItem(fieldPaths, filterOperator, value));
        }
        public override string ToString()
        {
            switch (OpType)
            {
                case CombinSymbol.AndItems:
                    return string.Join($" {Operator_And} ",
                        Items.TrimNotNull().Select(p => $"({p})"));
                case CombinSymbol.OrItems:
                    return string.Join($" {Operator_Or} ", Items.TrimNotNull().Select(p => $"({p})"));
                default:
                    return
                        $"{ValueToString(Left)} {FilterTypeToString(Operator)} {ValueToString(Right)}";
            }
            string ValueToString(ValueInfo p0)
            {
                return p0?.ToString() ?? "null";
            }
            string FilterTypeToString(Operator filterType)
            {
                return OperatorTypeCodeDictionary[filterType];
            }
        }

        public static FilterInfo Parse(string filterExpression) => Parse(filterExpression, CultureInfo.CurrentCulture);

        public static FilterInfo Parse(string filterExpression, CultureInfo cultureInfo)
        {
            throw new NotImplementedException();
            //return new QueryExpressionParser(cultureInfo).ParseFilter(filterExpression);
        }
        public static FilterInfo CreateItem(string fieldPaths, Operator filterType, object value)
        {
            return new FilterInfo()
            {
                OpType = CombinSymbol.SingleItem,
                Operator = filterType,
                Left = ValueInfo.Parse(fieldPaths),
                Right = new ValueInfo { NavigatePaths= new List<ValuePath> { new ValuePath { IsConstant = true, ConstantValue = value } } }
            };
        }

        public static FilterInfo CreateOr(params FilterInfo[] items)
        {
            return new FilterInfo { Items = items.TrimNotNull().ToList(), OpType = CombinSymbol.OrItems };
        }

        public static FilterInfo CreateAnd(params FilterInfo[] items)
        {
            return new FilterInfo { Items = items.TrimNotNull().ToList(), OpType = CombinSymbol.AndItems };
        }


        public FilterInfo Not()
        {
            if (OpType == CombinSymbol.SingleItem)
            {
                Operator = ~Operator;
                return this;
            }
            else if (OpType == CombinSymbol.AndItems)
            {
                // AndCondition current = this as AndCondition;
                FilterInfo oc = new FilterInfo() { OpType = CombinSymbol.OrItems, Items = new List<FilterInfo>() };
                foreach (var v in this.Items)
                {
                    oc.Items.Add(v.Not());
                }

                return oc;
            }
            else
            {
                FilterInfo oc = new FilterInfo() { OpType = CombinSymbol.AndItems, Items = new List<FilterInfo>() };
                foreach (var v in this.Items)
                {
                    oc.Items.Add(v.Not());
                }

                return oc;
            }
        }

    }
}
