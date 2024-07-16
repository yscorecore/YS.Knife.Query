using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace YS.Knife.Query.ExpressionConverters
{
    internal class ImplicitNumberConverter : IExpressionConverter
    {
        //https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/numeric-conversions
        /*
sbyte	short, int, long, float, double, decimal, nint
byte	short, ushort, int, uint, long, ulong, float, double, decimal, nint,  nuint
short	int, long, float, double, or decimal,  nint
ushort	int, uint, long, ulong, float, double,  decimal, nint,  nuint
int	long, float, double,  decimal, nint
uint	long, ulong, float, double,  decimal,  nuint
long	float, double,  decimal
ulong	float, double,  decimal
float	double
nint	long, float, double,  decimal
nuint	ulong, float, double,  decimal
         */
        private static Dictionary<Type, HashSet<Type>> defaultConverters = new Dictionary<Type, HashSet<Type>>
        {
            [typeof(sbyte)] = new HashSet<Type> { typeof(short), typeof(int), typeof(long), typeof(float), typeof(double), typeof(decimal), typeof(nint) },
            [typeof(byte)] = new HashSet<Type> { typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal), typeof(nint), typeof(nuint) },
            [typeof(short)] = new HashSet<Type> { typeof(int), typeof(long), typeof(float), typeof(double), typeof(decimal), typeof(nint) },
            [typeof(ushort)] = new HashSet<Type> { typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal), typeof(nint), typeof(nuint) },
            [typeof(int)] = new HashSet<Type> { typeof(long), typeof(float), typeof(double), typeof(decimal), typeof(nint) },
            [typeof(uint)] = new HashSet<Type> { typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal), typeof(nuint) },
            [typeof(long)] = new HashSet<Type> { typeof(float), typeof(double), typeof(decimal) },
            [typeof(ulong)] = new HashSet<Type> { typeof(float), typeof(double), typeof(decimal) },
            [typeof(float)] = new HashSet<Type> { typeof(double) },
            [typeof(nint)] = new HashSet<Type> { typeof(long), typeof(float), typeof(double), typeof(decimal) },
            [typeof(nuint)] = new HashSet<Type> { typeof(ulong), typeof(float), typeof(double), typeof(decimal) },
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
