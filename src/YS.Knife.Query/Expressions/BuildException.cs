using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YS.Knife.Query.Expressions
{

    [Serializable]
    public class BuildException : Exception
    {
        public BuildException() { }
        public BuildException(string message) : base(message) { }
        public BuildException(string message, Exception inner) : base(message, inner) { }
        protected BuildException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
