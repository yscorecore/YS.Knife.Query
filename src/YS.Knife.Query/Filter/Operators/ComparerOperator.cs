using System;
using System.Collections;
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
            return new ValueExpressionDesc
            {
                Expression = DoOperatorAction(left, right),
                ValueType = typeof(bool)
            };
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

    internal abstract class NotOperator : IExpressionOperator
    {
        public Operator Operator { get; set; }

        public bool IsNot { get; set; }

        public ValueExpressionDesc CreatePredicateExpression(OperatorExpressionContext context)
        {
            Debug.Assert(context.Left.ValueType == context.Left.ValueType);
            var exp = DoOperatorAction(context.Left, context.Right);
            if (IsNot)
            {
                exp = Expression.Not(exp);
            }
            return new ValueExpressionDesc
            {
                ValueType = typeof(bool),
                Expression = exp
            };
        }
        protected abstract Expression DoOperatorAction(ValueExpressionDesc left, ValueExpressionDesc right);

    }
    internal class StringOperator : NotOperator
    {
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
            return new ValueExpressionDesc
            {
                Expression = Expression.Call(null, method, right.Expression, left.Expression),
                ValueType = typeof(bool)
            };
        }
    }

    internal class BetweenOperator : IExpressionOperator
    {
        public BetweenOperator(Operator @operator)
        {
            this.Operator = @operator;
        }
        private IExpressionOperator LessThanOrEquals = new ComparerOperator(Operator.LessThanOrEqual);
        private IExpressionOperator GreatThanOrEquals = new ComparerOperator(Operator.GreaterThanOrEqual);
        public Operator Operator { get; private set; }
        public ValueExpressionDesc CreatePredicateExpression(OperatorExpressionContext context)
        {
            var right = context.Right;
            if (!right.IsConstant)
            {
                throw new Exception($"The right value for operator '{Operator}' should be constant.");
            }
            var rightValue = (right.Expression as ConstantExpression).Value;
            if (TryGetStartValueAndEndValue(rightValue, out var startValue, out var endValue))
            {
                ValueExpressionDesc startExpression = CreateStartExpression(context.Left, startValue);
                ValueExpressionDesc endExpression = CreateEndExpression(context.Left, endValue);
                if (startExpression != null && endExpression != null)
                {
                    return new ValueExpressionDesc
                    {
                        ValueType = typeof(bool),
                        Expression = Expression.AndAlso(startExpression.Expression, endExpression.Expression)
                    };
                }
                else
                {
                    return startExpression ?? endExpression ?? new ValueExpressionDesc
                    {
                        IsConstant = true,
                        ValueType = typeof(bool),
                        Expression = Expression.Constant(true)
                    };
                }
            }
            else
            {
                throw new Exception($"Invalud right value for operator '{Operator}'");
            }

        }
        private bool TryGetStartValueAndEndValue(object value, out object start, out object end)
        {
            start = null;
            end = null;
            if (value == null)
            {
                return true;
            }
            if (value is ICollection collection)
            {
              
                if (collection.Count != 2)
                {
                    return false;
                }
                var items = new object[2];
                collection.CopyTo(items, 0);
                start = items[0];
                end = items[1];
                return true;
            }
            else
            {
                return false;
            }

        }
        private ValueExpressionDesc CreateStartExpression(ValueExpressionDesc left, object startValue)
        {
            if (startValue == null)
            {
                return null;
            }
            var newCompareContext = new OperatorExpressionContext
            {
                Left = left,
                Right =  ValueExpressionDesc.FromValue(startValue)
            };
            return GreatThanOrEquals.CreatePredicateExpression(newCompareContext);
        }
        private ValueExpressionDesc CreateEndExpression(ValueExpressionDesc left, object endValue)
        {
            if (endValue == null)
            {
                return null;
            }
            var newCompareContext = new OperatorExpressionContext
            {
                Left = left,
                Right = ValueExpressionDesc.FromValue(endValue)
            };
            return LessThanOrEquals.CreatePredicateExpression(newCompareContext);

        }
    }
}
