using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace YS.Knife.Query.IntegrationTest
{
    public class OrderByExtensionsTest
    {
        [Fact]
        public void DoOrderByNull()
        {
            var source = CreateTestData();
            var data = source.DoOrderBy(null).ToList();
            data.Count.Should().Be(3);
        }

        [Fact]
        public void DoOrderBy0Column()
        {
            var source = CreateTestData();
            var data = source.DoOrderBy(new OrderByInfo()).ToList();
            data.Count.Should().Be(3);
        }


        [Fact]
        public void DoOrderByValue2Asc()
        {
            var source = CreateTestData();
            var data = source.DoOrderBy(new OrderByInfo(new OrderByItem { OrderByType = OrderByType.Asc, NavigatePaths = new List<ValuePath> { new ValuePath { Name = nameof(Entity.Value2) } } })).ToList();
            var val = data.Select(p => p.Value2).ToArray();
            val.Should().BeEquivalentTo(new int[] { 8, 9, 10 });
        }


        [Fact]
        public void DoOrderByValue2Desc()
        {
            var source = CreateTestData();
            var data = source.DoOrderBy(new OrderByInfo(new OrderByItem { OrderByType = OrderByType.Desc, NavigatePaths = new List<ValuePath> { new ValuePath { Name = nameof(Entity.Value2) } } })).ToList();
            var val = data.Select(p => p.Value2).ToArray();
            val.Should().BeEquivalentTo(new int[] { 10, 9, 8 });
        }


        [Fact]
        public void DoOrderByValue1AscAndValue2Asc()
        {
            var source = CreateTestData();
            var data = source.DoOrderBy(
                new OrderByInfo(
                    new OrderByItem { OrderByType = OrderByType.Asc, NavigatePaths = new List<ValuePath> { new ValuePath { Name = nameof(Entity.Value) } } },
                    new OrderByItem { OrderByType = OrderByType.Asc, NavigatePaths = new List<ValuePath> { new ValuePath { Name = nameof(Entity.Value2) } } })).ToList();
            var val = data.Select(p => p.Value2).ToArray();
            val.Should().BeEquivalentTo(new int[] { 10, 8, 9 });
        }
        [Fact]
        public void DoOrderByValue1AscAndValue2Desc()
        {
            var source = CreateTestData();
            var data = source.DoOrderBy(
                new OrderByInfo(
                    new OrderByItem { OrderByType = OrderByType.Asc, NavigatePaths = new List<ValuePath> { new ValuePath { Name = nameof(Entity.Value) } } },
                    new OrderByItem { OrderByType = OrderByType.Desc, NavigatePaths = new List<ValuePath> { new ValuePath { Name = nameof(Entity.Value2) } } })).ToList();
            var val = data.Select(p => p.Value2).ToArray();
            val.Should().BeEquivalentTo(new int[] { 10, 9, 8 });
        }

        [Fact]
        public void DoOrderByValue1DescAndValue2Asc()
        {
            var source = CreateTestData();
            var data = source.DoOrderBy(
                new OrderByInfo(
                    new OrderByItem { OrderByType = OrderByType.Desc, NavigatePaths = new List<ValuePath> { new ValuePath { Name = nameof(Entity.Value) } } },
                    new OrderByItem { OrderByType = OrderByType.Asc, NavigatePaths = new List<ValuePath> { new ValuePath { Name = nameof(Entity.Value2) } } })).ToList();
            var val = data.Select(p => p.Value2).ToArray();
            val.Should().BeEquivalentTo(new int[] { 8, 9, 10 });
        }
        [Fact]
        public void DoOrderByValue1DescAndValue2Desc()
        {
            var source = CreateTestData();
            var data = source.DoOrderBy(
                new OrderByInfo(
                    new OrderByItem { OrderByType = OrderByType.Desc, NavigatePaths = new List<ValuePath> { new ValuePath { Name = nameof(Entity.Value) } } },
                    new OrderByItem { OrderByType = OrderByType.Desc, NavigatePaths = new List<ValuePath> { new ValuePath { Name = nameof(Entity.Value2) } } })).ToList();
            var val = data.Select(p => p.Value2).ToArray();
            val.Should().BeEquivalentTo(new int[] { 9, 8, 10 });
        }

        private IQueryable<Entity> CreateTestData()
        {
            var array = new Entity[]
                {
                    new Entity{ Value=1,Value2=10},
                    new Entity{ Value=2,Value2=8},
                    new Entity{ Value=2,Value2=9},
                };
            return array.AsQueryable();

        }

        public record Entity
        {
            public int Value { get; set; }
            public int Value2 { get; set; }

        }
    }
}
