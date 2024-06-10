using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Collections.Immutable;
using System.Threading.Tasks;
using YS.Knife.Query;

namespace System.Linq
{
    public static class AggExtensions
    {
        private static IImmutableList<PropertyInfo> TempRecordProperties =
            typeof(TempRecord).GetProperties().ToImmutableList();
        public static Dictionary<string, object> DoAgg<T>(this IQueryable<T> source, AggInfo aggInfo) 
        {
            if (aggInfo?.Items?.Count > 0)
            {
                var tempRecord = source.GroupBy(p => 1).Select(p => new TempRecord()).FirstOrDefault();
                if (tempRecord != null)
                {

                }
                return null;
            }
            else
            {
                return null;
            }
            
        }

        internal class TempRecord
        {
            public object Column0 { get; set; }
            public object Column1 { get; set; }
            public object Column2 { get; set; }
            public object Column3 { get; set; }
            public object Column4 { get; set; }
            public object Column5 { get; set; }
            public object Column6 { get; set; }
            public object Column7 { get; set; }
            public object Column8 { get; set; }
            public object Column9 { get; set; }
            public object Column10 { get; set; }
        }
    }
}
