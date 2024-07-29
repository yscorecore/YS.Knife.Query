using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace YS.Knife.Query.Expressions
{
    public record ValueExecuteContext
    {
        private ValueExecuteContext()
        {

        }
        public ValueExecuteContext(ParameterExpression p)
        {
            this.Parameters = new List<ParameterExpression>
            {
                p
            };
            this.LastParameter = p;
        }
        public List<ParameterExpression> Parameters { get; private set; }

        public ParameterExpression LastParameter { get; private set; }
        public ValueExpressionDesc LastExpression { get; internal set; }

        public ValueExecuteContext Fork(ParameterExpression sub)
        {
            return new ValueExecuteContext
            {
                Parameters = new List<ParameterExpression>(Parameters.Concat(new ParameterExpression[] { sub })),
                LastParameter = sub,
                LastExpression = null
            };
        }
        public int Deepth { get => this.Parameters.Count; }
        public ValueExecuteContext Pop()
        {
            if (Parameters.Count == 0)
            {
                throw new QueryExpressionBuildException("context deep is zero.");
            }
            else
            {
                var parameters = this.Parameters.SkipLast(1).ToList();
                return new ValueExecuteContext
                {
                    Parameters = parameters,
                    LastExpression = null,
                    LastParameter = parameters.Last(),
                };
            }
        }


    }


}
