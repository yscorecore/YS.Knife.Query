using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using YS.Knife.Query.Expressions;

namespace YS.Knife.Query.Functions
{
    internal class Now : StaticFunction<DateTime>
    {
        public Now() : base(() => DateTime.Now)
        {
        }
        //private static PropertyInfo NowProperty = typeof(DateTime)
        //    .GetProperty(nameof(DateTime.Now), BindingFlags.Static | BindingFlags.Public);
        //protected override ValueExpressionDesc ExecuteStaticFunction(FunctionContext context)
        //{
        //    if (context.Arguments.Length > 0)
        //    {
        //        throw new Exception($"function '{nameof(Now)}' should has 0 argument.");
        //    }
        //    //DateTime.Now.AddYears()
        //    var expression = Expression.Property(null, NowProperty);
        //    return ValueExpressionDesc.FromExpression(expression);
        //}
    }
}
