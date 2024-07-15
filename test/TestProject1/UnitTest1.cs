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
            var source = GetTestSources();
            var res = source.Select(p => (int?)p.Age);
        }
        [Fact]
        public void Test5()
        {
            var str = @"sbyte	byte, ushort, uint, ulong, or nuint
byte	sbyte
short	sbyte, byte, ushort, uint, ulong, or nuint
ushort	sbyte, byte, or short
int	sbyte, byte, short, ushort, uint, ulong, or nuint
uint	sbyte, byte, short, ushort, int, or nint
long	sbyte, byte, short, ushort, int, uint, ulong, nint, or nuint
ulong	sbyte, byte, short, ushort, int, uint, long, nint, or nuint
float	sbyte, byte, short, ushort, int, uint, long, ulong, decimal, nint, or nuint
double	sbyte, byte, short, ushort, int, uint, long, ulong, float, decimal, nint, or nuint
decimal	sbyte, byte, short, ushort, int, uint, long, ulong, float, double, nint, or nuint
nint	sbyte, byte, short, ushort, int, uint, ulong, or nuint
nuint	sbyte, byte, short, ushort, int, uint,";
            using var sr = new StringReader(str);
            do
            {
                var line = sr.ReadLine();
                if (!string.IsNullOrEmpty(line))
                {
                    var items = line.Split(new string[] { "\t", ",", " " }, System.StringSplitOptions.RemoveEmptyEntries);
                    var from = items[0];
                    string resline = $"[typeof({items[0]})] = new HashSet<Type> {{ {string.Join(", ", items.Skip(1).Where(p => p != "or").Select(p => $"typeof({p})"))} }},";
                    testOutputHelper.WriteLine(resline);
                }
                else
                {
                    break;
                }
            } while (true);


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
