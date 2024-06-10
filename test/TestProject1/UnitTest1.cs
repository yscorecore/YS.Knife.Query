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

            var abd = source.GroupBy(p => 1).Select(t => new
            {
                maxAge = t.Max(p => p.Age)
            }).FirstOrDefault();
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
