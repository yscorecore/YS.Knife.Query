using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace YS.Knife.Query.Expressions
{
    internal record Func2LambdaExpressionDesc
    {
        public LambdaExpression Lambda { get; set; }
        public Type FuncType { get; set; }
        public Type SourceType { get; set; }
        public Type ValueType { get; set; }
    }
}
