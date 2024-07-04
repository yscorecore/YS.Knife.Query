using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace YS.Knife.Query.Expressions
{
    internal partial class LambdaUtils
    {
        public static Func2LambdaExpressionDesc CreateFunc2Lambda(Type fromType, List<ValuePath> paths, bool caseNullable)
        {
            var p = Expression.Parameter(fromType, "p");
            var lastValueExpression = new ValueExpressionDesc
            {
                Expression = p,
                ValueType = fromType
            };
            foreach (var vp in paths)
            {
                var context = new ValueNavigateContext
                {
                    RootExpression = p,
                    ParentExpression = lastValueExpression.Expression,
                    ParentType = lastValueExpression.ValueType,
                    ValuePath = vp,
                };
                lastValueExpression = ExecuteValuePath(context);
            }
            if (Nullable.GetUnderlyingType(lastValueExpression.ValueType) == null && caseNullable)
            {
                var nullableType = typeof(Nullable<>).MakeGenericType(lastValueExpression.ValueType);
                lastValueExpression = new ValueExpressionDesc
                {
                    ValueType = nullableType,
                    Expression = Expression.Convert(lastValueExpression.Expression, nullableType)
                };

            }
            var funcType = typeof(Func<,>).MakeGenericType(fromType, lastValueExpression.ValueType);
            return new Func2LambdaExpressionDesc
            {
                SourceType = fromType,
                ValueType = lastValueExpression.ValueType,
                FuncType = funcType,
                Lambda = Expression.Lambda(funcType, lastValueExpression.Expression, p)
            };
        }


        private static ValueExpressionDesc ExecuteValuePath(ValueNavigateContext context)
        {
            if (context.ValuePath.IsFunction)
            {
                return ExecuteFunctionPath(context);
            }
            else
            {
                return ExecuteMemberPath(context);
            }
        }
        private static ValueExpressionDesc ExecuteMemberPath(ValueNavigateContext context)
        {
            var property = PropertyFinder.GetProertyOrField(context.ParentType, context.ValuePath.Name);
            var expression = Expression.Property(context.ParentExpression, property);
            return new ValueExpressionDesc
            {
                Expression = expression,
                ValueType = property.PropertyType
            };
        }
        private static ValueExpressionDesc ExecuteFunctionPath(ValueNavigateContext context)
        {
            throw new NotSupportedException();
        }

    }
}
