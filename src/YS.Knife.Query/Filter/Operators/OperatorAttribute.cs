using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YS.Knife.Query.Filter.Operators
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    internal class OperatorAttribute : Attribute
    {
        public OperatorAttribute(Operator code)
        {
            this.Code = code;
        }
        public Operator Code { get; set; }
    }
}
