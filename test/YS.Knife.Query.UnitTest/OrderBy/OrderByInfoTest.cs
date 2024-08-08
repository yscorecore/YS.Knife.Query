using System.ComponentModel;
using FluentAssertions;
using Xunit;
using YS.Knife.Query;

namespace YS.Knife.Data.Query
{

    public class OrderByInfoTest
    {


        [Fact]
        public void ShouldGetExpectedStringWhenToString()
        {
            OrderByInfo order = OrderByInfo.Parse("Name,Age.desc()");
            order.ToString().Should().Be("Name.asc(),Age.desc()");
        }

        [Fact]
        public void ShouldGetExpectedOrderByInfoWhenParseFromString()
        {
            var orderInfo = OrderByInfo.Parse("Name, Age.desc() ,Address.asc() ");
            orderInfo.HasItems().Should().BeTrue();
            orderInfo.Items.Count.Should().Be(3);
            AssertOrderItem(orderInfo.Items[0], "Name", OrderByType.Asc);
            AssertOrderItem(orderInfo.Items[1], "Age", OrderByType.Desc);
            AssertOrderItem(orderInfo.Items[2], "Address", OrderByType.Asc);
        }

        private void AssertOrderItem(OrderByItem orderItem, string field, OrderByType orderType)
        {
            string.Join(".", orderItem.NavigatePaths).Should().Be(field);
            orderItem.OrderByType.Should().Be(orderType);
        }

        [Fact]
        public void ShouldCanConvertToString()
        {
            var converter = TypeDescriptor.GetConverter(typeof(OrderByInfo));
            converter.CanConvertTo(typeof(string)).Should().Be(true);
            OrderByInfo orderItem = OrderByInfo.Parse("Field.Desc()");
            converter.ConvertTo(orderItem, typeof(string)).Should().Be("Field.desc()");
        }
        [Fact]
        public void ShouldCanConvertFromString()
        {
            var converter = TypeDescriptor.GetConverter(typeof(OrderByInfo));
            converter.CanConvertFrom(typeof(string)).Should().Be(true);
            converter.ConvertFrom("Field.desc(),Field2.desc()").Should().BeOfType<OrderByInfo>();
        }
    }
}
