using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YS.Knife.Query
{
    public class ValuePath
    {
        public string Name { get; set; }
        public bool IsFunction { get; set; }
        public object[] FunctionArgs { get; set; }
        public override string ToString()
        {
            if (IsFunction)
            {
                var args = new List<string>();
                if (FunctionArgs != null)
                {
                    args.AddRange(FunctionArgs.Where(p => p != null).Select(p => p?.ToString()));
                }
                return $"{Name}({string.Join(", ", args)})";
            }
            else
            {
                return $"{Name}";
            }


        }
    }

}
