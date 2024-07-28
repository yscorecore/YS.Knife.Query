using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using YS.Knife.Query.Expressions;

namespace YS.Knife.Query.Filter.Operators
{
    internal class ComparerOperator : IExpressionOperator
    {
        public ComparerOperator(Operator @operator)
        {
            this.Operator = @operator;
        }
        public Operator Operator { get; set; }
        private static MethodInfo StaticStringCompare = typeof(string)
            .GetMethod(nameof(string.Compare),
            BindingFlags.Static | BindingFlags.Public, null,
            new Type[] { typeof(string), typeof(string) }, null);
        public ValueExpressionDesc CreatePredicateExpression(OperatorExpressionContext context)
        {
            var (left, right) = LambdaUtils.ConvertToSameType(context.Left, context.Right);
            return ValueExpressionDesc.FromExpression(DoOperatorAction(left, right));
        }
        private Func<Expression, Expression, BinaryExpression> GetExpressionMethod()
        {
            return Operator switch
            {
                Operator.GreaterThan => Expression.GreaterThan,
                Operator.GreaterThanOrEqual => Expression.GreaterThanOrEqual,
                Operator.LessThan => Expression.LessThan,
                Operator.LessThanOrEqual => Expression.LessThanOrEqual,
                _ => throw new InvalidOperationException("invalid operator type"),
            };

        }
        private Expression DoOperatorAction(ValueExpressionDesc left, ValueExpressionDesc right)
        {
            Debug.Assert(left.ValueType == right.ValueType);
            if (left.ValueType == typeof(string))
            {
                return GetExpressionMethod()(Expression.Call(null, StaticStringCompare,
                     left.Expression, right.Expression), Expression.Constant(0));
            }
            else if (left.ValueType.SupportsComparisonOperators())
            {
                return GetExpressionMethod()(left.Expression, right.Expression);
            }
            else
            {
                throw new Exception($"Operator '{Operator}' does not support type '{left.ValueType.FullName}'.");
            }

        }
    }

    internal abstract class ArrayOperator : IExpressionOperator
    {
        public Operator Operator { get; }
        public ValueExpressionDesc CreatePredicateExpression(OperatorExpressionContext context)
        {
            var (left, right) = LambdaUtils.ConvertToSameItemType(context.Left, context.Right);
            return DoOperatorAction(left, right);
        }
        protected abstract ValueExpressionDesc DoOperatorAction(ValueExpressionDesc left, ValueExpressionDesc right);
    }
    internal class InOperator : ArrayOperator
    {
        private static readonly MethodInfo ContainsMethod = typeof(Enumerable).GetMethods()
         .Where(p => p.Name == nameof(Enumerable.Contains))
         .Where(p => p.GetParameters().Length == 2)
         .Single();
        protected override ValueExpressionDesc DoOperatorAction(ValueExpressionDesc left, ValueExpressionDesc right)
        {
            var method = ContainsMethod.MakeGenericMethod(left.ValueType);
            return ValueExpressionDesc.FromExpression(Expression.Call(null, method, right.Expression, left.Expression));
        }
    }
}
