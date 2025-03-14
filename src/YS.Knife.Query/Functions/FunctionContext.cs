﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YS.Knife.Query.Expressions;

namespace YS.Knife.Query.Functions
{
    public record FunctionContext
    {
        public string Name { get; internal set; }
        public ValueInfo[] Arguments { get; internal set; }
        public ValueExecuteContext ExecuteContext { get; internal set; }
    }
}
