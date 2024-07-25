using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace TestProject1
{
    public class UnitTest1
    {
        private readonly ITestOutputHelper testOutputHelper;

        public UnitTest1(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }
        [Fact]
        public void Test1()
        {
            var source = new Entity<int?, int?> { Val1 = 1, Val2 = 2 };
            var query = new Entity<int?, int?>[] { source }.AsQueryable();
            var res = query.Where(p => p.Val1.Value.CompareTo(p.Val2.Value) < 0);
        }

        [Fact]
        public void Test2()
        {
            var source = GetTestSources();
            var query = source.GroupBy(p => 1).Select(t => new
            {
                Column0 = t.Count(p => true)
            });
            var res = query.ToList();
        }
        [Fact]
        public void Test3()
        {
            var source = GetTestSources();
            var result = source.DoAgg(new YS.Knife.Query.AggInfo
            {
                Items = new List<YS.Knife.Query.AggItem>
             {
                 new YS.Knife.Query.AggItem
                 {
                      AggType= YS.Knife.Query.AggType.Sum,
                        NavigatePaths= new List<YS.Knife.Query.ValuePath>
                        {
                         new YS.Knife.Query.ValuePath{ Name="Age"}
                        }
                 }
             }

            });
            var r = result.ToList();
        }

        public IQueryable<User> GetTestSources()
        {
            var sources = new User[]
            {
            new User{ Name="zhangsan",Age=11},
            new User{ Name="lisi",Age=12}
            };
            return sources.AsQueryable();
        }

        public record User
        {
            public string Name { get; set; }
            public int Age { get; set; }

        }
        public record Entity<T1, T2>
        {
            public T1 Val1 { get; set; }
            public T2 Val2 { get; set; }
        }
    }
}
