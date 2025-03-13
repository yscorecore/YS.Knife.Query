using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YS.Knife.Query.ValueConverters
{
    internal class EnumConverter : IValueConverter
    {
        internal HashSet<Type> numberTypes = new HashSet<Type>()
        {
           typeof(int),
           typeof(sbyte),
           typeof(short),
           typeof(long),
           typeof(uint),
           typeof(byte),
           typeof(ushort),
           typeof(ulong),
           typeof(char),
           typeof(bool)
        };
        public virtual bool CanConvertTo(Type fromType, Type toType)
        {
            return numberTypes.Contains(fromType) && toType.IsEnum;
        }

        public virtual object Convert(object fromValue, Type toType)
        {
            return Enum.ToObject(toType, fromValue);
        }
    }
}
