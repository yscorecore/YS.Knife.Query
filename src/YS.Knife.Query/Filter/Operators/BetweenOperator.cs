using System;
using System.Collections;
using System.Linq.Expressions;
using YS.Knife.Query.Expressions;

namespace YS.Knife.Query.Filter.Operators
{
    internal class BetweenOperator : NotResultOperator
    {
        public BetweenOperator(Operator @operator) : base(@operator)
        {
        }
        private readonly IExpressionOperator LessThanOrEquals = new ComparerOperator(Operator.LessThanOrEqual);
        private readonly IExpressionOperator GreatThanOrEquals = new ComparerOperator(Operator.GreaterThanOrEqual);
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
                Right = ValueExpressionDesc.FromValue(startValue)
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

        protected override Expression DoOperatorAction(ValueExpressionDesc left, ValueExpressionDesc right)
        {
            if (!right.IsConstant)
            {
                throw new Exception($"The right value for operator '{Operator}' should be constant.");
            }
            var rightValue = (right.Expression as ConstantExpression).Value;
            if (TryGetStartValueAndEndValue(rightValue, out var startValue, out var endValue))
            {
                ValueExpressionDesc startExpression = CreateStartExpression(left, startValue);
                ValueExpressionDesc endExpression = CreateEndExpression(left, endValue);
                if (startExpression != null && endExpression != null)
                {
                    return Expression.AndAlso(startExpression.Expression, endExpression.Expression);
                }
                else
                {
                    return (startExpression?.Expression) ?? (endExpression?.Expression) ?? Expression.Constant(true);
                }
            }
            else
            {
                throw new Exception($"Invalud right value for operator '{Operator}'");
            }
        }
    }
}
