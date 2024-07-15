using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using YS.Knife.Query.Converters;

namespace YS.Knife.Query.UnitTest
{
    public class ExpressionConverterTest
    {
        [Theory]
        [MemberData(nameof(GetShouldConvertSourceTypeToTargetTypeData))]
        public void ShouldConvertSourceTypeToTargetType(Type from, object fromValue, Type to)
        {
            var converter = ExpressionConverters.GetConverter(from, to);
            converter.Should().NotBeNull();
            var exp = System.Linq.Expressions.Expression.Constant(fromValue, from);
            var convertExp = converter.Convert(exp, to);
            var convertLambda = System.Linq.Expressions.Expression.Lambda(convertExp);
            var convertFunc = convertLambda.Compile();
            _ = convertFunc.DynamicInvoke();

        }
        public static IEnumerable<object[]> GetShouldConvertSourceTypeToTargetTypeData()
        {
            return new List<object[]>
            {
                new object[]{ typeof(int),1,typeof(int?) },
                new object[]{ typeof(Sub),new Sub(),typeof(Parent)},
                new object[]{ typeof(Sub),new Sub(),typeof(ISub)},

                new object[]{ typeof(int),1,typeof(long) },
                new object[]{ typeof(int),1,typeof(double) },
                new object[]{ typeof(int),1,typeof(float) },
                new object[]{ typeof(int),1,typeof(decimal) },
                new object[]{ typeof(int),1,typeof(double?) },
                new object[]{ typeof(int?),1,typeof(double?) },
                new object[]{ typeof(int?),null,typeof(double?) },
                new object[]{ typeof(int),1,typeof(short) },
                new object[]{ typeof(int?),1,typeof(short?) },
                new object[]{ typeof(int?),null,typeof(short?) },
                new object[]{ typeof(int),1,typeof(string) },
                new object[]{ typeof(int?),1,typeof(string) },
                new object[]{ typeof(int?),null,typeof(string) },
                new object[]{ typeof(string),"1",typeof(double) },
                new object[]{ typeof(string),"1",typeof(double?) },
                new object[]{ typeof(string),"2024-07-11",typeof(DateTime) },
                new object[]{ typeof(string),"2024-07-11",typeof(DateTime?) },
                new object[]{ typeof(string),"2024-07-11 21:51:00",typeof(DateTime) },
                new object[]{ typeof(string),"2024-07-11 21:51:00",typeof(DateTime?) },
                new object[]{ typeof(string),"2024-07-11T21:51:00",typeof(DateTime) },
                new object[]{ typeof(string),"2024-07-11T21:51:00",typeof(DateTime?) },
                new object[]{ typeof(DateTime),DateTime.Parse("2024-07-14"),typeof(string) },
                new object[]{ typeof(DateTime?),DateTime.Parse("2024-07-14"),typeof(string) },

                //new object[]{ typeof(string),"2024-07-11",typeof(DateTimeOffset) },
                //new object[]{ typeof(string),"2024-07-11",typeof(DateTimeOffset?) },
                //new object[]{ typeof(string),"2024-07-11 21:51:00",typeof(DateTimeOffset) },
                //new object[]{ typeof(string),"2024-07-11 21:51:00",typeof(DateTimeOffset?) },
                //new object[]{ typeof(string),"2024-07-11T21:51:00",typeof(DateTimeOffset) },
                //new object[]{ typeof(string),"2024-07-11T21:51:00",typeof(DateTimeOffset?) },
                new object[]{ typeof(DateTimeOffset), DateTimeOffset.Parse("2024-07-14"),typeof(string) },
                new object[]{ typeof(DateTimeOffset?), DateTimeOffset.Parse("2024-07-14"),typeof(string) },
            };
        }

        public class Parent
        {

        }
        public interface ISub
        {

        }
        public class Sub : Parent, ISub
        {

        }
    }
}
