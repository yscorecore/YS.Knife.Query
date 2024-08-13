using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YS.Knife.Query
{
    public class SelectItem
    {
        public bool Exclude { get; set; }
        public string Name { get; set; }

        // SubItems for sub object type or collection type
        public List<SelectItem> SubItems { get; set; }

        // subFilter,Order,Limit only for collection type

        public FilterInfo CollectionFilter { get; set; }

        public OrderByInfo CollectionOrderBy { get; set; }

        public LimitInfo CollectionLimit { get; set; }

        public override string ToString()
        {
            //source{where(a=b),orderby(a.asc(),b.desc()),limit(1,3)}(a,b)
            StringBuilder sb = new StringBuilder();
            if(Exclude)
            {
                sb.Append('-');
            }
            sb.Append(this.Name);
            if (CollectionFilter != null || CollectionOrderBy != null || CollectionLimit != null)
            {
                sb.Append("{");
                string collectionInfo = string.Join(',',
                    new string[] {
                        LimitInfoToString(CollectionLimit),
                        OrderInfoToString(CollectionOrderBy),
                        FilterInfoToString( CollectionFilter),
                }.Where(p => p != null));
                sb.Append(collectionInfo);
                sb.Append("}");
            }
            if (SubItems != null)
            {
                sb.Append($"({string.Join(',', SubItems.Where(p => p != null).Select(p => p.ToString()))})");
            }
            return sb.ToString();
        }
        private string LimitInfoToString(LimitInfo limitInfo)
        {
            return limitInfo != null ? $"limit({limitInfo})" : null;
        }
        private string OrderInfoToString(OrderByInfo orderInfo)
        {
            return orderInfo != null ? $"orderby({orderInfo})" : null;
        }
        private string FilterInfoToString(FilterInfo filterInfo)
        {
            return filterInfo != null ? $"where({filterInfo})" : null;
        }
    }
}
