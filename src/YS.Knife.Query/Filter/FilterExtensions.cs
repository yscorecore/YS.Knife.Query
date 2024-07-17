using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using YS.Knife.Query.Expressions;
using YS.Knife.Query.Filter.Operators;

namespace YS.Knife.Query
{
    public static class FilterExtensions
    {
        public static IQueryable<T> DoFilter<T>(this IQueryable<T> source, FilterInfo filter)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            return filter == null ? source : source.Where(CreateFilterLambdaExpression<T>(filter));
        }

        private static Expression<Func<T, bool>> CreateFilterLambdaExpression<T>(FilterInfo filterInfo)
        {
            var p = Expression.Parameter(typeof(T), "p");
            var expression = CreateFilterExpression(p, filterInfo);
            return Expression.Lambda<Func<T, bool>>(expression, p);
        }


        private static Expression CreateFilterExpression(ParameterExpression p, FilterInfo filterInfo)
        {
            if (filterInfo == null)
            {
                return Expression.Constant(true);
            }
            else
            {
                return CreateCombinGroupsFilterExpression(p, filterInfo);
            }
        }
        private static Expression CreateCombinGroupsFilterExpression(ParameterExpression p, FilterInfo filterInfo)
        {
            return filterInfo.OpType switch
            {
                CombinSymbol.AndItems => CreateAndConditionFilterExpression(p, filterInfo),
                CombinSymbol.OrItems => CreateOrConditionFilterExpression(p, filterInfo),
                _ => CreateSingleItemFilterExpression(p, filterInfo)
            };
        }

        private static Expression CreateOrConditionFilterExpression(ParameterExpression p, FilterInfo orGroupFilterInfo)
        {
            Expression current = null;
            foreach (FilterInfo item in orGroupFilterInfo.Items.TrimNotNull())
            {
                var next = CreateCombinGroupsFilterExpression(p, item);
                current = current == null ? next : Expression.OrElse(current, next);
            }
            return current ?? Expression.Constant(true);
        }
        private static Expression CreateAndConditionFilterExpression(ParameterExpression p, FilterInfo andGroupFilterInfo)
        {
            Expression current = null;
            foreach (FilterInfo item in andGroupFilterInfo.Items.TrimNotNull())
            {
                var next = CreateCombinGroupsFilterExpression(p, item);
                current = current == null ? next : Expression.AndAlso(current, next);
            }
            return current ?? Expression.Constant(true);
        }

        private static Expression CreateSingleItemFilterExpression(ParameterExpression p, FilterInfo singleItemFilter)
        {
            // TODO 处理导航属性为空的情况
            var context = new OperatorExpressionContext
            {
                Left = LambdaUtils.ExecuteValueInfo(p, singleItemFilter.Left),
                Right = LambdaUtils.ExecuteValueInfo(p, singleItemFilter.Right)
            };
            var expressionOperator = GetExpressionOperator(singleItemFilter.Operator);
            var expressionDesc = expressionOperator.CreatePredicateExpression(context);
            if (expressionDesc.ValueType != typeof(bool))
            {
                throw new QueryExpressionBuildException($"the return type of the operate '{singleItemFilter.Operator}' should be bool.");
            }
            return expressionDesc.Expression;
        }

        private static Dictionary<Operator, IExpressionOperator> supportOperators = new Dictionary<Operator, IExpressionOperator>
        {
            [Operator.Equals]= new OperatorEquals(),
        };

        private static IExpressionOperator GetExpressionOperator(Operator operatorType)
        {
            return supportOperators[operatorType];
        }

        private static void ConvertToSameType(OperatorExpressionContext context)
        { 
            
        }
    }
}
