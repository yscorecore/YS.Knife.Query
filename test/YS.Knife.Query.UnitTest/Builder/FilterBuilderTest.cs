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

        [Fact]
        public void ShouldCreateFilter_Array_Contains()
        {
            var values = new[] { "123", "456" };
            var filter = Query.Builder.CreateFilter<SimpleObject<string>>(p => values.Contains(p.Prop));
            ShouldBe(filter, "Prop in [\"123\",\"456\"]");
        }

        [Fact]
        public void ShouldCreateFilter_List_Contains()
        {
            var values = new List<int> { 1, 2, 3 };
            var filter = Query.Builder.CreateFilter<SimpleObject<int>>(p => values.Contains(p.Prop));
            ShouldBe(filter, "Prop in [1,2,3]");
        }

        [Fact]
        public void ShouldCreateFilter_NewArray_Contains()
        {
            var filter = Query.Builder.CreateFilter<SimpleObject<string>>(p => new[] { "a", "b" }.Contains(p.Prop));
            ShouldBe(filter, "Prop in [\"a\",\"b\"]");
        }

        [Fact]
        public void ShouldCreateFilter_Not_Array_Contains()
        {
            var values = new[] { "123", "456" };
            var filter = Query.Builder.CreateFilter<SimpleObject<string>>(p => !values.Contains(p.Prop));
            ShouldBe(filter, "Prop nin [\"123\",\"456\"]");
        }

        [Fact]
        public void ShouldCreateFilter_Array_Contains_With_AndAlso()
        {
            var values = new[] { "123", "456" };
            var filter = Query.Builder.CreateFilter<SimpleObject<string>>(p => values.Contains(p.Prop) && p.Prop != null);
            ShouldBe(filter, "(Prop in [\"123\",\"456\"]) and (Prop != null)");
        }

        [Fact]
        public void ShouldCreateFilter_CollectionProperty_Contains_Constant()
        {
            var filter = Query.Builder.CreateFilter<CollectionObject<string>>(p => p.Items.Contains("a"));
            ShouldBe(filter, "\"a\" in Items");
        }

        [Fact]
        public void ShouldCreateFilter_CollectionProperty_Contains_Path()
        {
            var filter = Query.Builder.CreateFilter<CollectionObject<string>>(p => p.Items.Contains(p.Prop));
            ShouldBe(filter, "Prop in Items");
        }

        [Fact]
        public void ShouldCreateFilter_Any_NoArgs()
        {
            var filter = Query.Builder.CreateFilter<CollectionObject<string>>(p => p.Items.Any());
            ShouldBe(filter, "Items.Any() == true");
        }

        [Fact]
        public void ShouldCreateFilter_Any_NoArgs_Negated()
        {
            var filter = Query.Builder.CreateFilter<CollectionObject<string>>(p => !p.Items.Any());
            ShouldBe(filter, "Items.Any() != true");
        }

        [Fact]
        public void ShouldCreateFilter_Count_NoArgs_Greater()
        {
            var filter = Query.Builder.CreateFilter<CollectionObject<string>>(p => p.Items.Count() > 0);
            ShouldBe(filter, "Items.Count() > 0");
        }

        [Fact]
        public void ShouldCreateFilter_Count_NoArgs_Equals()
        {
            var filter = Query.Builder.CreateFilter<CollectionObject<string>>(p => p.Items.Count() == 3);
            ShouldBe(filter, "Items.Count() == 3");
        }

        [Fact]
        public void ShouldCreateFilter_Any_AndAlso_Count()
        {
            var filter = Query.Builder.CreateFilter<CollectionObject<string>>(p => p.Items.Any() && p.Items.Count() < 10);
            ShouldBe(filter, "(Items.Any() == true) and (Items.Count() < 10)");
        }

        [Fact]
        public void ShouldCreateFilter_Any_With_Lambda()
        {
            var filter = Query.Builder.CreateFilter<ParentObject>(p => p.Children.Any(c => c.Age > 18));
            ShouldBe(filter, "Children.Any(Age > 18) == true");
        }

        [Fact]
        public void ShouldCreateFilter_Any_With_Lambda_Negated()
        {
            var filter = Query.Builder.CreateFilter<ParentObject>(p => !p.Children.Any(c => c.Age > 18));
            ShouldBe(filter, "Children.Any(Age > 18) != true");
        }

        [Fact]
        public void ShouldCreateFilter_Any_With_Lambda_AndAlso()
        {
            var filter = Query.Builder.CreateFilter<ParentObject>(p => p.Children.Any(c => c.Age > 18 && c.Name != null));
            ShouldBe(filter, "Children.Any((Age > 18) and (Name != null)) == true");
        }

        [Fact]
        public void ShouldCreateFilter_Count_With_Lambda_Greater()
        {
            var filter = Query.Builder.CreateFilter<ParentObject>(p => p.Children.Count(c => c.Age > 18) > 0);
            ShouldBe(filter, "Children.Count(Age > 18) > 0");
        }

        [Fact]
        public void ShouldCreateFilter_Count_With_Lambda_Equals()
        {
            var filter = Query.Builder.CreateFilter<ParentObject>(p => p.Children.Count(c => c.Age > 18) == 3);
            ShouldBe(filter, "Children.Count(Age > 18) == 3");
        }

        [Fact]
        public void ShouldCreateFilter_Any_With_NestedProperty_In_Lambda()
        {
            var filter = Query.Builder.CreateFilter<ParentObject>(p => p.Children.Any(c => c.Address.City == "北京"));
            ShouldBe(filter, "Children.Any(Address.City == \"北京\") == true");
        }

        [Fact]
        public void ShouldCreateFilter_Any_With_OrElse_In_Lambda()
        {
            var filter = Query.Builder.CreateFilter<ParentObject>(p => p.Children.Any(c => c.Age < 12 || c.Age > 60));
            ShouldBe(filter, "Children.Any((Age < 12) or (Age > 60)) == true");
        }

        [Fact]
        public void ShouldCreateFilter_Any_With_Lambda_AndAlso_Outer()
        {
            var filter = Query.Builder.CreateFilter<ParentObject>(p => p.Children.Any(c => c.Age > 18) && p.Name != null);
            ShouldBe(filter, "(Children.Any(Age > 18) == true) and (Name != null)");
        }

        public record SimpleObject<T>
        {
            public T Prop { get; set; }
        }

        public record CollectionObject<T>
        {
            public T Prop { get; set; }
            public T[] Items { get; set; }
        }

        public record ChildObject
        {
            public string Name { get; set; }
            public int Age { get; set; }
            public AddressObject Address { get; set; }
        }

        public record AddressObject
        {
            public string City { get; set; }
        }

        public record ParentObject
        {
            public string Name { get; set; }
            public List<ChildObject> Children { get; set; }
        }
        public enum TestEnum
        {
            Val = 1
        }
    }
}
