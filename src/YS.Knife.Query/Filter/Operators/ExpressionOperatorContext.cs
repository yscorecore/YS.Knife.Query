using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace YS.Knife.Query.Filter.Operators
{
    internal record ExpressionOperatorContext
    {
        public ValueInfo Left { get; set; }
        public ValueInfo Right { get; set; }
        public ParameterExpression Parameter { get; set; }

        public FilterInfo FilterInfo { get; set; }
    }
}
