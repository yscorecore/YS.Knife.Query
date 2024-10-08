﻿using System;
using System.ComponentModel.DataAnnotations;

namespace YS.Knife.Query
{
    [Serializable]
    public record LimitQueryInfo : QueryInfo, ILimitInfo
    {
        [Range(0, int.MaxValue)]
        public int Offset { get; set; } = 0;
        [Range(0, 10000)]
        public int Limit { get; set; } = 10000;

        [MaxLength(1024)]
        public string Agg { get; set; }
    }
}
