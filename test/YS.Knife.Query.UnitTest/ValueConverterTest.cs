using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;
using YS.Knife.Query.ValueConverters;

namespace YS.Knife.Query.UnitTest
{
    public class ValueConverterTest
    {
        [Theory]
        [MemberData(nameof(GetShouldConvertSourceTypeToTargetTypeData))]
        public void ShouldConvertSourceTypeToTargetType(Type from, object fromValue, Type to)
        {
            var converter = ValueConverterFactory.GetConverter(from, to);
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
                new object[]{ typeof(DateTime?),DateTime.Parse("2024-07-14"),typeof(string)  },
                new object[]{ typeof(Guid),Guid.Parse("5F5CBD67-D6F9-41D0-8111-1D6C70BFFAEF"),typeof(string)  },
                new object[]{ typeof(Guid?),Guid.Parse("5F5CBD67-D6F9-41D0-8111-1D6C70BFFAEF"),typeof(string)  },
                new object[]{ typeof(Guid?),null,typeof(string)  },
                new object[]{ typeof(string), "5F5CBD67-D6F9-41D0-8111-1D6C70BFFAEF", typeof(Guid)},
                new object[]{ typeof(string), "5F5CBD67-D6F9-41D0-8111-1D6C70BFFAEF", typeof(Guid?)},
                new object[]{ typeof(string), null, typeof(Guid?)},
                new object[]{ typeof(string),"2024-07-11",typeof(DateTimeOffset) },
                new object[]{ typeof(string),"2024-07-11",typeof(DateTimeOffset?) },
                new object[]{ typeof(string),"2024-07-11 21:51:00",typeof(DateTimeOffset) },
                new object[]{ typeof(string),"2024-07-11 21:51:00",typeof(DateTimeOffset?) },
                new object[]{ typeof(string),"2024-07-11T21:51:00",typeof(DateTimeOffset) },
                new object[]{ typeof(string),"2024-07-11T21:51:00",typeof(DateTimeOffset?) },
                new object[]{ typeof(DateTimeOffset), DateTimeOffset.Parse("2024-07-14"),typeof(string) },
                new object[]{ typeof(DateTimeOffset?), DateTimeOffset.Parse("2024-07-14"),typeof(string) },
               // new object[]{ typeof(int), 0,typeof(Enum1) },
              //  new object[]{ typeof(Enum1), Enum1.Value1,typeof(int) },
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

        public enum Enum1
        {
            Value0 = 0,
            Value1 = 1,
        }
    }
}
