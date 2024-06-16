using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace YS.Knife.Query.IntegrationTest
{
    public class AggExtensionsTest
    {
        [Fact]
        public void ShouldSumValues()
        {
            var source = GetTestSources<double>();
            var res = source.DoAgg(new AggInfo
            {
                Items = new List<AggItem>
                 {
                     new AggItem
                     {
                          AggType= AggType.Sum,
                          NavigatePaths= new List<ValuePath>
                          {
                             new ValuePath{ Name= nameof(Entity<double>.Value)}
                          }
                     }
                 }
            });
            res.First().Value.Should().Be(source.Select(p => p.Value).Sum());
        }


        public IQueryable<Entity<T>> GetTestSources<T>()
        {
            var faker = new Bogus.Faker<Entity<T>>()
                .RuleFor(p => p.Value, f => System.Convert.ChangeType(f.Random.Double(), typeof(T)));
            var data = faker.Generate(10);
            return data.AsQueryable();
        }

        public record Entity<T>
        {
            public T Value { get; set; }

        }
    }
}
