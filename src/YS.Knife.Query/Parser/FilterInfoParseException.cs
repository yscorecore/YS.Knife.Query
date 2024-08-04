using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YS.Knife.Query.Parser
{
    public class FilterInfoParseException : Exception
    {
        public FilterInfoParseException()
        {
        }

        public FilterInfoParseException(string message) : base(message)
        {
        }

        public FilterInfoParseException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
