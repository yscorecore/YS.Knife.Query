using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YS.Knife.Query.Expressions;

namespace YS.Knife.Query.Functions.Specials
{
    internal class Exists : InstanceFunction
    {
        protected override ValueExpressionDesc ExecuteInstanceFunction(FunctionContext context)
        {
            return (context.Arguments?.Length??0)
                switch
                {
                    0 => default(ValueExpressionDesc),
                    1 => default(ValueExpressionDesc),
                    _ => throw new Exception($"The argument count for function '{context.Name}' not match.")
                };
        }
    }

}
