using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YS.Knife.Query.Expressions;

namespace YS.Knife.Query.Functions
{
    public interface IFunction
    {
        ValueExpressionDesc Execute(FunctionContext context);
    }
}
