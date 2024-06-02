using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YS.Knife.Query.Limit
{
    public interface IPagedList : ILimitList
    {
        long TotalCount { get; }
        bool CanToPage => this.Limit > 0 && this.Offset % this.Limit == 0;
        int PageSize => this.Limit;
        int PageIndex => this.Limit > 0 ? this.Offset / this.Limit + 1 : 1;
    }

    public interface IPagedList<T> : IPagedList, ILimitList<T>
    {

    }
    public interface IPagedList<T,S> : IPagedList, ILimitList<T,S>
    {

    }
}
