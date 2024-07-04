using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YS.Knife.Query
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    internal class OperatorCodeAttribute : Attribute
    {
        public OperatorCodeAttribute(string code)
        {
            this.Code = code;
        }
        public string Code { get; set; }
    }
}
