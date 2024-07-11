using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;
using YS.Knife.Query.Converters;

namespace YS.Knife.Query.UnitTest
{
    public class ValueConverterTest
    {
        [Theory]
        [MemberData(nameof(GetShouldConvertSourceTypeToTargetTypeData))]
        public void ShouldConvertSourceTypeToTargetType(Type from, object fromValue, Type to)
        {
            var converter = ValueConverters.GetConverter(from, to);
            converter.Should().NotBeNull();
            _ = converter.Convert(fromValue, to);

        }
        public static IEnumerable<object[]> GetShouldConvertSourceTypeToTargetTypeData()
        {
            return new List<object[]>
            {
                new object[]{ typeof(int),1,typeof(int?) },
                new object[]{ typeof(Sub),new Sub(),typeof(Parent)},
                new object[]{ typeof(Sub),new Sub(),typeof(ISub)},
                new object[]{ typeof(int),1,typeof(double?) },
                new object[]{ typeof(int?),1,typeof(double?) },
                new object[]{ typeof(int?),null,typeof(double?) },
                new object[]{ typeof(int),1,typeof(short) },
                new object[]{ typeof(int?),1,typeof(short?) },
                 new object[]{ typeof(int?),null,typeof(short?) },
                new object[]{ typeof(string),"1",typeof(double) },
                new object[]{ typeof(string),"1",typeof(double?) },
                new object[]{ typeof(string),"2024-07-11",typeof(DateTime) },
                new object[]{ typeof(string),"2024-07-11",typeof(DateTime?) },
                new object[]{ typeof(string),"2024-07-11 21:51:00",typeof(DateTime) },
                new object[]{ typeof(string),"2024-07-11 21:51:00",typeof(DateTime?) },
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
