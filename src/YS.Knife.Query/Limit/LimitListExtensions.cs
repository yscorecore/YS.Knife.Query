using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YS.Knife.Query.Limit;

namespace System.Collections.Generic
{
    public static class LimitListExtensions
    {
        public static S ForEach<S, T>(this S source, Action<T> action)
            where S : ILimitList<T>
        {
            if (action != null)
            {
                source.Items.ForEach(action);
            }
            return source;
        }
    }
}
