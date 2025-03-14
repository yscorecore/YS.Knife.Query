﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YS.Knife.Query
{
    public class ValuePath
    {
        public object ConstantValue { get; set; }
        public bool IsConstant { get; set; }
        public string Name { get; set; }
        public bool IsFunction { get; set; }
        public ValueInfo[] FunctionArgs { get; set; }
        public override string ToString()
        {
            if (IsConstant)
            {
                return ValueInfo.ValueToString(ConstantValue);
            }
            else if (IsFunction)
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
