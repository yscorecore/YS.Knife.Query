﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YS.Knife.Query
{
    public interface ILimitInfo
    {
        int Offset { get; }
        int Limit { get; }
    }
}
