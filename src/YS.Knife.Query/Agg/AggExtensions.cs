using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using YS.Knife.Query;
using YS.Knife.Query.Expressions;

namespace System.Linq
{
    public static class AggExtensions
    {
        private static IImmutableList<PropertyInfo> TempRecordProperties =
            typeof(TempRecord).GetProperties().ToImmutableList();
        public static Dictionary<string, object> DoAgg<T>(this IQueryable<T> source, AggInfo aggInfo)
        {
            var items = aggInfo?.Items?.Where(p => p != null).ToList();
            if (items?.Count > TempRecordProperties.Count)
            {
                throw new NotSupportedException("max agg item count shoule not great than 64.");
            }
            if (items?.Count > 0)
            {
                var properties = TempRecordProperties.Take(items.Count).ToList();
                var group = source.GroupBy(p => 1);
                var tempRecordQuery = QueryFieldDaynmic(group, items, properties);
                var tempRecord = tempRecordQuery.FirstOrDefault();
                if (tempRecord != null)
                {
                    Dictionary<string, object> res = new Dictionary<string, object>();
                    for (int i = 0; i < items.Count; i++)
                    {
                        res[items[i].AggName ?? $"agg{i}"] = properties[i].GetValue(tempRecord);
                    }
                    return res;
                }
                else
                {
                    Dictionary<string, object> res = new Dictionary<string, object>();
                    for (int i = 0; i < items.Count; i++)
                    {
                        res[items[i].AggName ?? $"agg{i}"] = GetAggItemDefaultValue(items[i]);
                    }
                    return res;
                }

            }
            return null;
        }
        private static object GetAggItemDefaultValue(AggItem item)
        {
            return item.AggType switch
            {
                AggType.Sum => 0,
                AggType.Count => 0,
                AggType.DistinctCount => 0,
                _ => null,
            };

        }
        private static IQueryable<TempRecord> QueryFieldDaynmic<T>(IQueryable<IGrouping<int, T>> source, List<AggItem> items, List<PropertyInfo> properties)
        {
            var sourceExp = source.Expression;
            var method = QueryableMethodFinder.GetQuerybleSelect<IGrouping<int, T>, TempRecord>();
            var selectExp = Expression.Call(null, method, sourceExp, GetAssignExpression<T>(items, properties));
            return source.Provider.CreateQuery<TempRecord>(selectExp);
        }
        private static Expression<Func<IGrouping<int, T1>, TempRecord>> GetAssignExpression<T1>(List<AggItem> items, List<PropertyInfo> properties)
        {
            var p = Expression.Parameter(typeof(IGrouping<int, T1>));
            MemberBinding[] memberBindings = new MemberBinding[items.Count];
            for (int i = 0; i < items.Count; i++)
            {
                memberBindings[i] = CreateMemberBinding<T1>(items[i], properties[i], p);
            }
            var memberInitExpression = Expression.MemberInit(Expression.New(typeof(TempRecord).GetConstructor(Type.EmptyTypes)),
                memberBindings);

            return Expression.Lambda<Func<IGrouping<int, T1>, TempRecord>>(memberInitExpression, p);
        }
        private static MemberBinding CreateMemberBinding<T>(AggItem aggItem, PropertyInfo propertyInfo, Expression sourceExpression)
        {
            if (aggItem.AggType == AggType.Count)
            {
                var method = EnumerableMethodFinder.GetLongCount1(typeof(T));
                var valueExp = Expression.Call(null, method, sourceExpression);
                var castToObject = Expression.Convert(valueExp, typeof(object));
                return Expression.Bind(propertyInfo, castToObject);
            }
            else if (aggItem.AggType == AggType.DistinctCount)
            {
                var lambdaDesc = LambdaUtils.CreateFunc2Lambda(typeof(T), aggItem.NavigatePaths, false);
                var selectMethod = EnumerableMethodFinder.GetSelect2(typeof(T), lambdaDesc.ValueType);
                var selectExpression = Expression.Call(null, selectMethod, sourceExpression, lambdaDesc.Lambda);
                var distinctMethod = EnumerableMethodFinder.GetDistinct1(lambdaDesc.ValueType);
                var distinctExpression = Expression.Call(null, distinctMethod, selectExpression);
                var longCountmethod = EnumerableMethodFinder.GetLongCount1(lambdaDesc.ValueType);
                var valueExp = Expression.Call(null, longCountmethod, distinctExpression);
                var castToObject = Expression.Convert(valueExp, typeof(object));
                return Expression.Bind(propertyInfo, castToObject);
            }
            else if (aggItem.AggType == AggType.Sum)
            {
                var lambdaDesc = LambdaUtils.CreateFunc2Lambda(typeof(T), aggItem.NavigatePaths, false);
                var method = GetMethod(aggItem, typeof(T), lambdaDesc.ValueType);
                var valueExp = Expression.Call(null, method, sourceExpression, lambdaDesc.Lambda);
                var castToObject = Expression.Convert(valueExp, typeof(object));
                return Expression.Bind(propertyInfo, castToObject);
            }
            else
            {
                var lambdaDesc = LambdaUtils.CreateFunc2Lambda(typeof(T), aggItem.NavigatePaths, true);
                var method = GetMethod(aggItem, typeof(T), lambdaDesc.ValueType);
                var valueExp = Expression.Call(null, method, sourceExpression, lambdaDesc.Lambda);
                var castToObject = Expression.Convert(valueExp, typeof(object));
                return Expression.Bind(propertyInfo, castToObject);
            }


        }
        private static MethodInfo GetMethod(AggItem aggItem, Type type, Type returnType)
        {
            return aggItem.AggType switch
            {
                AggType.Sum => EnumerableMethodFinder.GetSumAgg2(type, returnType),
                AggType.Max => EnumerableMethodFinder.GetMaxAgg2(type, returnType),
                AggType.Min => EnumerableMethodFinder.GetMinAgg2(type, returnType),
                AggType.Avg => EnumerableMethodFinder.GetAverageAgg2(type, returnType),
                _ => throw new InvalidEnumArgumentException()
            };
        }
      
        public record TempRecord
        {
            public object Column0 { get; set; }
            public object Column1 { get; set; }
            public object Column2 { get; set; }
            public object Column3 { get; set; }
            public object Column4 { get; set; }
            public object Column5 { get; set; }
            public object Column6 { get; set; }
            public object Column7 { get; set; }
            public object Column8 { get; set; }
            public object Column9 { get; set; }
            public object Column10 { get; set; }
            public object Column11 { get; set; }
            public object Column12 { get; set; }
            public object Column13 { get; set; }
            public object Column14 { get; set; }
            public object Column15 { get; set; }
            public object Column16 { get; set; }
            public object Column17 { get; set; }
            public object Column18 { get; set; }
            public object Column19 { get; set; }
            public object Column20 { get; set; }
            public object Column21 { get; set; }
            public object Column22 { get; set; }
            public object Column23 { get; set; }
            public object Column24 { get; set; }
            public object Column25 { get; set; }
            public object Column26 { get; set; }
            public object Column27 { get; set; }
            public object Column28 { get; set; }
            public object Column29 { get; set; }
            public object Column30 { get; set; }
            public object Column31 { get; set; }
            public object Column32 { get; set; }
            public object Column33 { get; set; }
            public object Column34 { get; set; }
            public object Column35 { get; set; }
            public object Column36 { get; set; }
            public object Column37 { get; set; }
            public object Column38 { get; set; }
            public object Column39 { get; set; }
            public object Column40 { get; set; }
            public object Column41 { get; set; }
            public object Column42 { get; set; }
            public object Column43 { get; set; }
            public object Column44 { get; set; }
            public object Column45 { get; set; }
            public object Column46 { get; set; }
            public object Column47 { get; set; }
            public object Column48 { get; set; }
            public object Column49 { get; set; }
            public object Column50 { get; set; }
            public object Column51 { get; set; }
            public object Column52 { get; set; }
            public object Column53 { get; set; }
            public object Column54 { get; set; }
            public object Column55 { get; set; }
            public object Column56 { get; set; }
            public object Column57 { get; set; }
            public object Column58 { get; set; }
            public object Column59 { get; set; }
            public object Column60 { get; set; }
            public object Column61 { get; set; }
            public object Column62 { get; set; }
            public object Column63 { get; set; }
        }
    }
}
