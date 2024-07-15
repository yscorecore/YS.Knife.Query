using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace YS.Knife.Query.Converters
{
    internal class ExplicitNumberConverter:IExpressionConverter
    {
        /*
sbyte	byte, ushort, uint, ulong, or nuint
byte	sbyte
short	sbyte, byte, ushort, uint, ulong, or nuint
ushort	sbyte, byte, or short
int	sbyte, byte, short, ushort, uint, ulong, or nuint
uint	sbyte, byte, short, ushort, int, or nint
long	sbyte, byte, short, ushort, int, uint, ulong, nint, or nuint
ulong	sbyte, byte, short, ushort, int, uint, long, nint, or nuint
float	sbyte, byte, short, ushort, int, uint, long, ulong, decimal, nint, or nuint
double	sbyte, byte, short, ushort, int, uint, long, ulong, float, decimal, nint, or nuint
decimal	sbyte, byte, short, ushort, int, uint, long, ulong, float, double, nint, or nuint
nint	sbyte, byte, short, ushort, int, uint, ulong, or nuint
nuint	sbyte, byte, short, ushort, int, uint, long, or nint
         */

        private static Dictionary<Type, HashSet<Type>> defaultConverters = new Dictionary<Type, HashSet<Type>>
        {
            [typeof(sbyte)] = new HashSet<Type> { typeof(byte), typeof(ushort), typeof(uint), typeof(ulong), typeof(nuint) },
            [typeof(byte)] = new HashSet<Type> { typeof(sbyte) },
            [typeof(short)] = new HashSet<Type> { typeof(sbyte), typeof(byte), typeof(ushort), typeof(uint), typeof(ulong), typeof(nuint) },
            [typeof(ushort)] = new HashSet<Type> { typeof(sbyte), typeof(byte), typeof(short) },
            [typeof(int)] = new HashSet<Type> { typeof(sbyte), typeof(byte), typeof(short), typeof(ushort), typeof(uint), typeof(ulong), typeof(nuint) },
            [typeof(uint)] = new HashSet<Type> { typeof(sbyte), typeof(byte), typeof(short), typeof(ushort), typeof(int), typeof(nint) },
            [typeof(long)] = new HashSet<Type> { typeof(sbyte), typeof(byte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(ulong), typeof(nint), typeof(nuint) },
            [typeof(ulong)] = new HashSet<Type> { typeof(sbyte), typeof(byte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(nint), typeof(nuint) },
            [typeof(float)] = new HashSet<Type> { typeof(sbyte), typeof(byte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(decimal), typeof(nint), typeof(nuint) },
            [typeof(double)] = new HashSet<Type> { typeof(sbyte), typeof(byte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(decimal), typeof(nint), typeof(nuint) },
            [typeof(decimal)] = new HashSet<Type> { typeof(sbyte), typeof(byte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(nint), typeof(nuint) },
            [typeof(nint)] = new HashSet<Type> { typeof(sbyte), typeof(byte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(ulong), typeof(nuint) },
            [typeof(nuint)] = new HashSet<Type> { typeof(sbyte), typeof(byte), typeof(short), typeof(ushort), typeof(int), typeof(uint) },
        };
        public virtual bool CanConvertTo(Type fromType, Type toType)
        {
            if (defaultConverters.TryGetValue(fromType, out var hs))
            {
                return hs.Contains(toType);
            }
            return false;
        }

        public virtual Expression Convert(Expression expression, Type toType)
        {
            return Expression.Convert(expression, toType);

        }

    }
}
