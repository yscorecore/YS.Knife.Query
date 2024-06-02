using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YS.Knife.Query.Limit
{
    public interface ILimitList
    {
        int Offset { get; }
        int Limit { get; }
        bool HasNext { get; }
    }
    public interface ILimitList<T> : ILimitList
    {
        List<T> Items { get; }
    }
    public interface ILimitList<T, S> : ILimitList<T>
    { 
        S Summary { get; }
    }
}
