using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace TestProject1
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {

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
    }
}
