using System;
using System.Linq.Expressions;

namespace YS.Knife.Query.Expressions
{

    internal record ValueExpressionDesc
    {
        public Expression Expression { get; set; }
        public Type ValueType { get; set; }
        public bool IsConstant { get; set; }
    }

}
