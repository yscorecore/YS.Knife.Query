using System;
using System.Linq.Expressions;

namespace YS.Knife.Query.Expressions
{
    public record ValueExpressionDesc
    {
        private ValueExpressionDesc()
        {

        }
        public Expression Expression { get; set; }
        public Type ValueType { get; set; }
        public bool IsConstant { get; set; }
        public bool IsNull { get; set; }

        public static ValueExpressionDesc FromValue(object value, Type valueType = null)
        {
            var exp = valueType == null ? Expression.Constant(value) : Expression.Constant(value, valueType);
            return new ValueExpressionDesc
            {
                IsConstant = true,
                IsNull = value == null,
                Expression = exp,
                ValueType = exp.Type
            };
        }

        public static ValueExpressionDesc FromExpression(Expression expression)
        {
            return new ValueExpressionDesc
            {
                Expression = expression,
                ValueType = expression.Type
            };
        }
    }

}
