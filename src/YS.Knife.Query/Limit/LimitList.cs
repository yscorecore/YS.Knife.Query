using System;
using System.Collections.Generic;
using System.Linq;

namespace YS.Knife.Query
{
    [Serializable]
    public record LimitList<T> : ILimitList<T>
    {
        public LimitList()
        {

        }
        public LimitList(IEnumerable<T> items, int offset, int limit, bool hasNext)
        {
            this.Items = items?.ToList() ?? new List<T>();
            this.Offset = offset;
            this.Limit = limit;
            this.HasNext = hasNext;
        }
        public List<T> Items { get; set; }

        public int Offset { get; set; }

        public int Limit { get; set; }

        public bool HasNext { get; private set; }

    }

    [Serializable]
    public record LimitList<T, S> : LimitList<T>, ILimitList<T, S>
    {
        public S Summary { get; }
    }
}
