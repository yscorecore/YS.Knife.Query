using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            var valueExpression = ExecuteValuePaths(p, paths);
            if (Nullable.GetUnderlyingType(valueExpression.ValueType) == null && caseNullable)
            {
                var nullableType = typeof(Nullable<>).MakeGenericType(valueExpression.ValueType);
                valueExpression = new ValueExpressionDesc
                {
                    ValueType = nullableType,
                    Expression = Expression.Convert(valueExpression.Expression, nullableType)
                };

            }
            var funcType = typeof(Func<,>).MakeGenericType(fromType, valueExpression.ValueType);
            return new Func2LambdaExpressionDesc
            {
                SourceType = fromType,
                ValueType = valueExpression.ValueType,
                FuncType = funcType,
                Lambda = Expression.Lambda(funcType, valueExpression.Expression, p)
            };
        }

        public static ValueExpressionDesc ExecuteValueInfo(ParameterExpression p, ValueInfo value)
        {
            if (value.IsConstant)
            {
                return ExecuteConstantValue(p, value);
            }
            else
            {
                return ExecuteValuePaths(p, value.NavigatePaths);
            }
        }
        private static ValueExpressionDesc ExecuteValuePaths(ParameterExpression p, List<ValuePath> valuePaths)
        {
            return ExecuteValuePaths(new ValueNavigateContext(p), valuePaths);
        }
        private static ValueExpressionDesc ExecuteValuePaths(ValueNavigateContext context, List<ValuePath> valuePaths)
        {
            foreach (var vp in valuePaths)
            {
                context.LastExpression = ExecuteValuePath(context, vp);
            }
            return context.LastExpression;
        }
        private static ValueExpressionDesc ExecuteConstantValue(ParameterExpression p, ValueInfo value)
        {
            Debug.Assert(value.IsConstant);
            var exp = Expression.Constant(value.ConstantValue);
            return new ValueExpressionDesc
            {
                Expression = exp,
                ValueType = exp.Type,
                IsConstant = true,
            };
        }

        private static ValueExpressionDesc ExecuteValuePath(ValueNavigateContext context, ValuePath valuePath)
        {
            if (valuePath.IsFunction)
            {
                return ExecuteFunctionPath(context, valuePath);
            }
            else
            {
                return ExecuteMemberPath(context, valuePath);
            }
        }
        private static ValueExpressionDesc ExecuteMemberPath(ValueNavigateContext context, ValuePath valuePath)
        {
            var lastExpression = context.LastExpression;
            if (lastExpression != null)
            {
                var property = PropertyFinder.GetProertyOrField(context.LastExpression.ValueType, valuePath.Name);
                if (property == null)
                {
                    throw new QueryExpressionBuildException($"can not find member '{valuePath.Name}' from type '{lastExpression.ValueType.FullName}'.");
                }
                var expression = Expression.Property(lastExpression.Expression, property);
                return new ValueExpressionDesc
                {
                    Expression = expression,
                    ValueType = property.PropertyType
                };
            }
            else
            {
                do
                {
                    var property = PropertyFinder.GetProertyOrField(context.LastParameter.Type, valuePath.Name);
                    if (property != null)
                    {
                        var expression = Expression.Property(context.LastParameter, property);
                        return new ValueExpressionDesc
                        {
                            Expression = expression,
                            ValueType = property.PropertyType
                        };
                    }
                    else
                    {
                        context = context.Pop();
                        if (context.Deepth == 0)
                        {
                            throw new QueryExpressionBuildException($"can not find member '{valuePath.Name}' from parameters.");
                        }
                    }
                }
                while (true);
            }


        }
        private static ValueExpressionDesc ExecuteFunctionPath(ValueNavigateContext context, ValuePath valuePath)
        {
            throw new NotSupportedException();
        }

    }
}
