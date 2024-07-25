using System;
using System.Linq.Expressions;
using System.Reflection;
using YS.Knife.Query.Expressions;

namespace YS.Knife.Query.Filter.Operators
{
    internal class StringOperator : NotResultOperator
    {
        public StringOperator(bool isNot, Operator @operator) : base(isNot, @operator)
        {
        }

        protected override Expression DoOperatorAction(ValueExpressionDesc left, ValueExpressionDesc right)
        {
            return Expression.Call(left.Expression, GetExpressionMethod(), right.Expression);
        }

        private MethodInfo GetExpressionMethod()
        {
            return Operator switch
            {
                Operator.StartsWith => StringMethodFinder.StartsWith,
                Operator.EndsWith => StringMethodFinder.EndsWith,
                Operator.Contains => StringMethodFinder.Contains,
                Operator.NotStartsWith => StringMethodFinder.StartsWith,
                Operator.NotEndsWith => StringMethodFinder.EndsWith,
                Operator.NotContains => StringMethodFinder.Contains,
                _ => throw new InvalidOperationException("invalid operator type"),
            };

        }

        internal class StringMethodFinder
        {
            public static MethodInfo StartsWith = typeof(string)
                .GetMethod(nameof(string.StartsWith),
                BindingFlags.Instance | BindingFlags.Public, null,
                new Type[] { typeof(string) }, null);
            public static MethodInfo EndsWith = typeof(string)
                .GetMethod(nameof(string.EndsWith),
                BindingFlags.Instance | BindingFlags.Public, null,
                new Type[] { typeof(string) }, null);
            public static MethodInfo Contains = typeof(string)
                .GetMethod(nameof(string.Contains),
                BindingFlags.Instance | BindingFlags.Public, null,
                new Type[] { typeof(string) }, null);
        }

    }
}
