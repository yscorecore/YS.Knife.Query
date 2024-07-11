using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YS.Knife.Query.Converters
{
    internal class BasicConverter : IValueConverter
    {
        public virtual bool CanConvertTo(Type fromType, Type toType)
        {
            return typeof(IConvertible).IsAssignableFrom(fromType)
                 && typeof(IConvertible).IsAssignableFrom(toType);

        }

        public virtual object Convert(object fromValue, Type toType)
        {
            return System.Convert.ChangeType(fromValue, toType);
        }
    }
}
