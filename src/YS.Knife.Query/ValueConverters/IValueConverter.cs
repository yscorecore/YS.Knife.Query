using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YS.Knife.Query.ValueConverters
{
    internal interface IValueConverter
    {
        bool CanConvertTo(Type fromType, Type toType);
        object Convert(object fromValue, Type toType);

    }
}
