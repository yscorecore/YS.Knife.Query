using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace YS.Knife.Query.ExpressionConverters
{
    internal class EnumConverter : IExpressionConverter
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
            return numberTypes.Contains(toType) && fromType.IsEnum;
        }


        public virtual Expression Convert(Expression expression, Type toType)
        {
            return Expression.Convert(expression, toType);
        }
    }
}
