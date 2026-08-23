using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using YS.Knife.Query.Expressions;
using YS.Knife.Query.Filter;

namespace YS.Knife.Query.Functions.Specials
{
    internal class Any : InstanceFunction
    {
        protected override ValueExpressionDesc ExecuteInstanceFunction(FunctionContext context)
        {
            var sourceExpressionDesc = context.ExecuteContext.LastExpression;
            var sourceType = sourceExpressionDesc.ValueType;
            var elementType = sourceType.GetEnumerableItemType();

            switch (context.Arguments?.Length ?? 0)
            {
                case 0:
                    {
                        var method = EnumerableMethodFinder.GetAny1(elementType);
                        var anyExp = Expression.Call(null, method, sourceExpressionDesc.Expression);
                        return ValueExpressionDesc.FromExpression(anyExp);
                    }
                case 1:
                    {
                        var arg = context.Arguments[0];
                        var parameter = Expression.Parameter(elementType, "p");
                        var argContext = new ValueExecuteContext(parameter);
                        var predicateExpressionDesc = LambdaUtils.ExecuteValueInfo(argContext, arg);
                        var predicateLambda = Expression.Lambda(predicateExpressionDesc.Expression, parameter);
                        var method = EnumerableMethodFinder.GetAny2(elementType);
                        var anyExp = Expression.Call(null, method, sourceExpressionDesc.Expression, predicateLambda);
                        return ValueExpressionDesc.FromExpression(anyExp);
                    }
                default:
                    throw new Exception($"The argument count for function '{context.Name}' not match.");
            }
        }
    }
}
