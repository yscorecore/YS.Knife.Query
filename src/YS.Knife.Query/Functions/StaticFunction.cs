using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YS.Knife.Query.Expressions;

namespace YS.Knife.Query.Functions
{
    public abstract class StaticFunction : IFunction
    {
        public ValueExpressionDesc Execute(FunctionContext context)
        {
            if (context.ExecuteContext.LastExpression != null)
            {
                throw new Exception("static function should not has instance.");
            }
            return ExecuteStaticFunction(context);
        }
        protected abstract ValueExpressionDesc ExecuteStaticFunction(FunctionContext context);

    }
}
