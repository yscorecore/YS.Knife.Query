using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YS.Knife.Query.Expressions
{

    [Serializable]
    public class QueryExpressionBuildException : Exception
    {
        public QueryExpressionBuildException() { }
        public QueryExpressionBuildException(string message) : base(message) { }
        public QueryExpressionBuildException(string message, Exception inner) : base(message, inner) { }
        protected QueryExpressionBuildException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
