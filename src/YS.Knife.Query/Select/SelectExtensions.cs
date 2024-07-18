using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace YS.Knife.Query
{
    public static class SelectExtensions
    {
        public static IQueryable<T> DoSelect<T>(this IQueryable<T> source, SelectInfo selectInfo)
            where T : class, new()
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            return selectInfo?.Items?.Count > 0 ? source.Select(CreateSelectLambdaExpression<T>(selectInfo)) : source;
        }
        private static Expression<Func<T, T>> CreateSelectLambdaExpression<T>(SelectInfo selectInfo)
        {
            var p = Expression.Parameter(typeof(T), "p");
            var memberInitExpression = CreateSelectLambdaExpression(typeof(T), selectInfo.Items, p);
            return Expression.Lambda<Func<T, T>>(memberInitExpression, p);
        }

        private static MemberInitExpression CreateSelectLambdaExpression(Type type, List<SelectItem> items, Expression source)
        {
            List<MemberBinding> bindings = new List<MemberBinding>();
            foreach (var item in items)
            {
                var property = PropertyFinder.GetProertyOrField(type, item.Name);
                if (property == null)
                {
                    throw new Exception($"can not find property '{item.Name}' from type '{type.FullName}'");
                }
                if (!property.CanWrite)
                {
                    throw new Exception($"the property '{item.Name}' of type '{type.FullName}' can not writed.");
                }
                if (item.SubItems?.Count > 0)
                {
                    var newSubObject = CreateSelectLambdaExpression(property.PropertyType, item.SubItems, Expression.Property(source, property));
                    bindings.Add(Expression.Bind(property, newSubObject));
                }
                else
                {

                    bindings.Add(Expression.Bind(property, Expression.Property(source, property)));
                }
            }
            return Expression.MemberInit(Expression.New(type.GetConstructor(Type.EmptyTypes)), bindings);
        }

    }
}
