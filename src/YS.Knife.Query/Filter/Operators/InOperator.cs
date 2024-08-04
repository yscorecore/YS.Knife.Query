using System.Linq;
using System.Linq.Expressions;
using YS.Knife.Query.Expressions;

namespace YS.Knife.Query.Filter.Operators
{
    internal class InOperator : NotResultOperator
    {
        public InOperator(Operator @operator) : base(@operator)
        {
        }
        protected override Expression DoOperatorAction(ValueExpressionDesc left, ValueExpressionDesc right)
        {
            var (left2, right2) = LambdaUtils.ConvertToSameItemType(left, right);
            var rightItemType = TypeUtils.GetEnumerableItemType(right2.ValueType);
            var containsMethod = EnumerableMethodFinder.GetContains(rightItemType);
            return Expression.Call(null, containsMethod, right2.Expression, left2.Expression);
        }
    }
}
