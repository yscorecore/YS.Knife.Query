using System;
using System.Linq.Expressions;

namespace YS.Knife.Query.Expressions
{

    public record ValueNavigateContext
    {
        public Expression RootExpression { get; set; }
        public Expression ParentExpression { get; set; }
        public Type ParentType { get; set; }
        public ValuePath ValuePath { get; set; }

    }

}
