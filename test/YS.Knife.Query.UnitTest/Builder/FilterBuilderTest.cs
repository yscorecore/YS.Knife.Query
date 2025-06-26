using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using static YS.Knife.Query.UnitTest.Builder.FilterBuilderTest;

namespace YS.Knife.Query.UnitTest.Builder
{
    public class FilterBuilderTest
    {
        private void ShouldBe(string filter, string expected)
        {
            filter.Should().Be(expected);
            FilterInfo.Parse(filter).ToString().Should().Be(expected);
        }
        [Fact]
        public void ShouldCreateFilter_String_Equals()
        {
            var filter = Query.Builder.CreateFilter<SimpleObject<string>>(p => p.Prop == "123");
            ShouldBe(filter, "Prop == \"123\"");
        }

        [Fact]
        public void ShouldCreateFilter_String_Equals_Var()
        {
            var abc = "123";
            var filter = Query.Builder.CreateFilter<SimpleObject<string>>(p => p.Prop == abc);
            ShouldBe(filter, "Prop == \"123\"");
        }

        [Fact]
        public void ShouldCreateFilter_String_Equals_Null()
        {
            var filter = Query.Builder.CreateFilter<SimpleObject<string>>(p => p.Prop == null);
            ShouldBe(filter, "Prop == null");
        }

        [Fact]
        public void ShouldCreateFilter_String_Not_Equals()
        {
            var filter = Query.Builder.CreateFilter<SimpleObject<string>>(p => p.Prop != "123");
            ShouldBe(filter, "Prop != \"123\"");
        }

        [Fact]
        public void ShouldCreateFilter_String_GreatThan()
        {
            var filter = Query.Builder.CreateFilter<SimpleObject<string>>(p => p.Prop.CompareTo("123") > 0);
            ShouldBe(filter, "Prop.CompareTo(\"123\") > 0");
        }
        [Fact]
        public void ShouldCreateFilter_String_GreatThanOrEquals()
        {
            var filter = Query.Builder.CreateFilter<SimpleObject<string>>(p => p.Prop.CompareTo("123") >= 0);
            ShouldBe(filter, "Prop.CompareTo(\"123\") >= 0");
        }

        [Fact]
        public void ShouldCreateFilter_String_LessThan()
        {
            var filter = Query.Builder.CreateFilter<SimpleObject<string>>(p => p.Prop.CompareTo("123") < 0);
            ShouldBe(filter, "Prop.CompareTo(\"123\") < 0");
        }
        [Fact]
        public void ShouldCreateFilter_String_LessThanOrEquals()
        {
            var filter = Query.Builder.CreateFilter<SimpleObject<string>>(p => p.Prop.CompareTo("123") <= 0);
            ShouldBe(filter, "Prop.CompareTo(\"123\") <= 0");
        }
        [Fact]
        public void ShouldCreateFilter_String_StartsWith()
        {
            var filter = Query.Builder.CreateFilter<SimpleObject<string>>(p => p.Prop.StartsWith("123"));
            ShouldBe(filter, "Prop.StartsWith(\"123\") == true");
        }
        [Fact]
        public void ShouldCreateFilter_String_StartsWith_Binary()
        {
            var filter = Query.Builder.CreateFilter<SimpleObject<string>>(p => p.Prop.StartsWith("123") == true);
            ShouldBe(filter, "Prop.StartsWith(\"123\") == true");
        }
        [Fact]
        public void ShouldCreateFilter_String_EndsWith()
        {
            var filter = Query.Builder.CreateFilter<SimpleObject<string>>(p => p.Prop.EndsWith("123"));
            ShouldBe(filter, "Prop.EndsWith(\"123\") == true");
        }
        [Fact]
        public void ShouldCreateFilter_String_EndsWith_Binary()
        {
            var filter = Query.Builder.CreateFilter<SimpleObject<string>>(p => p.Prop.EndsWith("123") == true);
            ShouldBe(filter, "Prop.EndsWith(\"123\") == true");
        }

        [Fact]
        public void ShouldCreateFilter_String_Contains()
        {
            var filter = Query.Builder.CreateFilter<SimpleObject<string>>(p => p.Prop.Contains("123"));
            ShouldBe(filter, "Prop.Contains(\"123\") == true");
        }
        [Fact]
        public void ShouldCreateFilter_String_Contains_Binary()
        {
            var filter = Query.Builder.CreateFilter<SimpleObject<string>>(p => p.Prop.Contains("123") == true);
            ShouldBe(filter, "Prop.Contains(\"123\") == true");
        }
        [Fact]
        public void ShouldCreateFilter_String_NotContains()
        {
            var filter = Query.Builder.CreateFilter<SimpleObject<string>>(p => !p.Prop.Contains("123"));
            ShouldBe(filter, "Prop.Contains(\"123\") != true");
        }
        [Fact]
        public void ShouldCreateFilter_String_NotContains_Binary()
        {
            var filter = Query.Builder.CreateFilter<SimpleObject<string>>(p => p.Prop.Contains("123") == false);
            ShouldBe(filter, "Prop.Contains(\"123\") == false");
        }
        [Fact]
        public void ShouldCreateFilter_Enum_Equals()
        {
            var filter = Query.Builder.CreateFilter<SimpleObject<TestEnum>>(p => p.Prop == TestEnum.Val);
            ShouldBe(filter, "Prop == 1");
        }

        [Fact]
        public void ShouldCreateFilter_Enum_Equals_Var()
        {
            var abc = TestEnum.Val;
            var filter = Query.Builder.CreateFilter<SimpleObject<TestEnum>>(p => p.Prop == abc);
            ShouldBe(filter, "Prop == 1");
        }

        [Fact]
        public void ShouldCreateFilter_Two_Item_With_AndAlso()
        {
            var filter = Query.Builder.CreateFilter<SimpleObject<int>>(p => p.Prop > 1 && p.Prop < 10);
            ShouldBe(filter, "(Prop > 1) and (Prop < 10)");
        }
        [Fact]
        public void ShouldCreateFilter_Two_Item_With_AndAlso_With_Bracket()
        {
            var filter = Query.Builder.CreateFilter<SimpleObject<int>>(p => p.Prop > 1 && (p.Prop < 10));
            ShouldBe(filter, "(Prop > 1) and (Prop < 10)");
        }
        [Fact]
        public void ShouldCreateFilter_Two_Item_With_OrElse()
        {
            var filter = Query.Builder.CreateFilter<SimpleObject<int>>(p => p.Prop > 1 || p.Prop < 10);
            ShouldBe(filter, "(Prop > 1) or (Prop < 10)");
        }

        public record SimpleObject<T>
        {
            public T Prop { get; set; }
        }
        public enum TestEnum
        {
            Val = 1
        }
    }
}
