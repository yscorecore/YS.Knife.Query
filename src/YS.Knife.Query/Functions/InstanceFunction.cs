using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YS.Knife.Query.Expressions;

namespace YS.Knife.Query.Functions
{
    internal abstract class InstanceFunction: IFunction
    {
        public ValueExpressionDesc Execute(FunctionContext context)
        {
            if (context.ExecuteContext.LastExpression == null)
            {
                throw new Exception("instance function should has instance.");
            }
            return ExecuteInstanceFunction(context);
        }
        protected abstract ValueExpressionDesc ExecuteInstanceFunction(FunctionContext context);
        
    }
}
