using System;
using System.Linq.Expressions;
using System.Reflection;
using YS.Knife.Query.Expressions;

namespace YS.Knife.Query.Filter.Operators
{
    internal class StringOperator : NotResultOperator
    {
        public StringOperator(Operator @operator) : base(@operator)
        {
        }

        protected override Expression DoOperatorAction(ValueExpressionDesc left, ValueExpressionDesc right)
        {
            var (left2, right2) = LambdaUtils.ConvertToStringType(left, right);
            if (right2.IsNull || left2.IsNull) return Expression.Constant(false);
            return Expression.Call(left2.Expression, GetExpressionMethod(), right2.Expression);
        }

        private MethodInfo GetExpressionMethod()
        {
            return Operator switch
            {
                Operator.StartsWith or Operator.NotStartsWith => StringMethodFinder.StartsWith,
                Operator.EndsWith or Operator.NotEndsWith => StringMethodFinder.EndsWith,
                Operator.Contains or Operator.NotContains => StringMethodFinder.Contains,
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
