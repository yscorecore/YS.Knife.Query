using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace YS.Knife.Query.IntegrationTest
{
    public class AggExtensionsTest
    {
        private readonly ITestOutputHelper testOutputHelper;

        public AggExtensionsTest(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }
        [Theory]
        [InlineData(typeof(double), 0)]
        [InlineData(typeof(double), 10)]
        [InlineData(typeof(int), 0)]
        [InlineData(typeof(int), 10)]
        [InlineData(typeof(long), 0)]
        [InlineData(typeof(long), 10)]
        [InlineData(typeof(decimal), 0)]
        [InlineData(typeof(decimal), 10)]
        [InlineData(typeof(float), 0)]
        [InlineData(typeof(float), 10)]
        [InlineData(typeof(double?), 0)]
        [InlineData(typeof(double?), 10)]
        [InlineData(typeof(int?), 0)]
        [InlineData(typeof(int?), 10)]
        [InlineData(typeof(long?), 0)]
        [InlineData(typeof(long?), 10)]
        [InlineData(typeof(decimal?), 0)]
        [InlineData(typeof(decimal?), 10)]
        [InlineData(typeof(float?), 0)]
        [InlineData(typeof(float?), 10)]
        public void ShouldDoSumAggs(Type dataType, int sampleCount)
        {
            var method = this.GetType().GetMethod(nameof(ShouldSumValueInternal), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            var instanceMethod = method.MakeGenericMethod(dataType);
            instanceMethod.Invoke(this, new object[] { sampleCount });
        }
        [Theory]
        [InlineData(typeof(double), 0)]
        [InlineData(typeof(double), 10)]
        [InlineData(typeof(int), 0)]
        [InlineData(typeof(int), 10)]
        [InlineData(typeof(long), 0)]
        [InlineData(typeof(long), 10)]
        [InlineData(typeof(decimal), 0)]
        [InlineData(typeof(decimal), 10)]
        [InlineData(typeof(float), 0)]
        [InlineData(typeof(float), 10)]
        [InlineData(typeof(double?), 0)]
        [InlineData(typeof(double?), 10)]
        [InlineData(typeof(int?), 0)]
        [InlineData(typeof(int?), 10)]
        [InlineData(typeof(long?), 0)]
        [InlineData(typeof(long?), 10)]
        [InlineData(typeof(decimal?), 0)]
        [InlineData(typeof(decimal?), 10)]
        [InlineData(typeof(float?), 0)]
        [InlineData(typeof(float?), 10)]
        public void ShouldDoMaxAggs(Type dataType, int sampleCount)
        {
            var isNullable = Nullable.GetUnderlyingType(dataType) != null;
            var method = this.GetType().GetMethod(isNullable ? nameof(ShouldNullableAggValueInternal) : nameof(ShouldAggValueInternal), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            var instanceMethod = method.MakeGenericMethod(dataType);
            instanceMethod.Invoke(this, new object[] { sampleCount, AggType.Max, nameof(Queryable.Max) });
        }

        [Theory]
        [InlineData(typeof(double), 0)]
        [InlineData(typeof(double), 10)]
        [InlineData(typeof(int), 0)]
        [InlineData(typeof(int), 10)]
        [InlineData(typeof(long), 0)]
        [InlineData(typeof(long), 10)]
        [InlineData(typeof(decimal), 0)]
        [InlineData(typeof(decimal), 10)]
        [InlineData(typeof(float), 0)]
        [InlineData(typeof(float), 10)]
        [InlineData(typeof(double?), 0)]
        [InlineData(typeof(double?), 10)]
        [InlineData(typeof(int?), 0)]
        [InlineData(typeof(int?), 10)]
        [InlineData(typeof(long?), 0)]
        [InlineData(typeof(long?), 10)]
        [InlineData(typeof(decimal?), 0)]
        [InlineData(typeof(decimal?), 10)]
        [InlineData(typeof(float?), 0)]
        [InlineData(typeof(float?), 10)]
        public void ShouldGoMinAggs(Type dataType, int sampleCount)
        {
            var isNullable = Nullable.GetUnderlyingType(dataType) != null;
            var method = this.GetType().GetMethod(isNullable ? nameof(ShouldNullableAggValueInternal) : nameof(ShouldAggValueInternal), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            var instanceMethod = method.MakeGenericMethod(dataType);
            instanceMethod.Invoke(this, new object[] { sampleCount, AggType.Min, nameof(Queryable.Min) });
        }

        [Theory]
        [InlineData(typeof(double), 0)]
        [InlineData(typeof(double), 10)]
        [InlineData(typeof(int), 0)]
        [InlineData(typeof(int), 10)]
        [InlineData(typeof(long), 0)]
        [InlineData(typeof(long), 10)]
        [InlineData(typeof(decimal), 0)]
        [InlineData(typeof(decimal), 10)]
        [InlineData(typeof(float), 0)]
        [InlineData(typeof(float), 10)]
        [InlineData(typeof(double?), 0)]
        [InlineData(typeof(double?), 10)]
        [InlineData(typeof(int?), 0)]
        [InlineData(typeof(int?), 10)]
        [InlineData(typeof(long?), 0)]
        [InlineData(typeof(long?), 10)]
        [InlineData(typeof(decimal?), 0)]
        [InlineData(typeof(decimal?), 10)]
        [InlineData(typeof(float?), 0)]
        [InlineData(typeof(float?), 10)]
        public void ShouldGoAvgAggs(Type dataType, int sampleCount)
        {
            var isNullable = Nullable.GetUnderlyingType(dataType) != null;
            var method = this.GetType().GetMethod(isNullable ? nameof(ShouldNullableAvgValueInternal) : nameof(ShouldAvgValueInternal), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            var instanceMethod = method.MakeGenericMethod(dataType);
            instanceMethod.Invoke(this, new object[] { sampleCount, AggType.Avg, nameof(Queryable.Average) });
        }



        #region Count
        [Theory]
        [InlineData(typeof(string), 0)]
        [InlineData(typeof(string), 10)]
        [InlineData(typeof(object), 0)]
        [InlineData(typeof(object), 10)]
        [InlineData(typeof(double), 0)]
        [InlineData(typeof(double), 10)]
        [InlineData(typeof(int), 0)]
        [InlineData(typeof(int), 10)]
        [InlineData(typeof(long), 0)]
        [InlineData(typeof(long), 10)]
        [InlineData(typeof(decimal), 0)]
        [InlineData(typeof(decimal), 10)]
        [InlineData(typeof(float), 0)]
        [InlineData(typeof(float), 10)]
        [InlineData(typeof(double?), 0)]
        [InlineData(typeof(double?), 10)]
        [InlineData(typeof(int?), 0)]
        [InlineData(typeof(int?), 10)]
        [InlineData(typeof(long?), 0)]
        [InlineData(typeof(long?), 10)]
        [InlineData(typeof(decimal?), 0)]
        [InlineData(typeof(decimal?), 10)]
        [InlineData(typeof(float?), 0)]
        [InlineData(typeof(float?), 10)]
        public void ShouldDoCountAggs(Type dataType, int sampleCount)
        {
            var method = this.GetType().GetMethod(nameof(ShouldDoCountAggsInternal), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            var instanceMethod = method.MakeGenericMethod(dataType);
            instanceMethod.Invoke(this, new object[] { sampleCount });



        }
        void ShouldDoCountAggsInternal<T>(int caseCount)
        {
            var source = GetTestSources<T>(caseCount, false);
            var res = source.DoAgg(new AggInfo
            {
                Items = new List<AggItem>
                 {
                     new AggItem
                     {
                          AggType= AggType.Count,
                          NavigatePaths= new List<ValuePath>
                          {
                             new ValuePath{ Name= nameof(Entity<T>.Value)}
                          }
                     }
                 }
            });

            var expectedRes = source.Count();
            var actualRes = res.First().Value;
            testOutputHelper.WriteLine($"execute Count, expected value is {expectedRes}, actual value is {actualRes}. ");
            actualRes.Should().Be(expectedRes);
        }
        #endregion

        #region DistinctCount
        [Theory]
        [InlineData(typeof(double), 0)]
        [InlineData(typeof(double), 10)]
        [InlineData(typeof(int), 0)]
        [InlineData(typeof(int), 10)]
        [InlineData(typeof(long), 0)]
        [InlineData(typeof(long), 10)]
        [InlineData(typeof(decimal), 0)]
        [InlineData(typeof(decimal), 10)]
        [InlineData(typeof(float), 0)]
        [InlineData(typeof(float), 10)]
        [InlineData(typeof(double?), 0)]
        [InlineData(typeof(double?), 10)]
        [InlineData(typeof(int?), 0)]
        [InlineData(typeof(int?), 10)]
        [InlineData(typeof(long?), 0)]
        [InlineData(typeof(long?), 10)]
        [InlineData(typeof(decimal?), 0)]
        [InlineData(typeof(decimal?), 10)]
        [InlineData(typeof(float?), 0)]
        [InlineData(typeof(float?), 10)]
        public void ShouldDistinctCountAggs(Type dataType, int sampleCount)
        {
            var method = this.GetType().GetMethod(nameof(ShouldDoDistinctCountAggsInternal), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            var instanceMethod = method.MakeGenericMethod(dataType);
            instanceMethod.Invoke(this, new object[] { sampleCount });



        }
        void ShouldDoDistinctCountAggsInternal<T>(int caseCount)
        {
            var source = GetRepeadTestSources<T>(caseCount, true);
            var res = source.DoAgg(new AggInfo
            {
                Items = new List<AggItem>
                 {
                     new AggItem
                     {
                          AggType= AggType.DistinctCount,
                          NavigatePaths= new List<ValuePath>
                          {
                             new ValuePath{ Name= nameof(Entity<T>.Value)}
                          }
                     }
                 }
            });

            var expectedRes = source.Select(p => p.Value).Distinct().Count();
            var actualRes = res.First().Value;
            testOutputHelper.WriteLine($"execute DistinctCount, expected value is {expectedRes}, actual value is {actualRes}. ");
            actualRes.Should().Be(expectedRes);
        }

        #endregion

        private void ShouldSumValueInternal<T>(int caseCount)
        {
            var source = GetTestSources<T>(caseCount);
            var res = source.DoAgg(new AggInfo
            {
                Items = new List<AggItem>
                 {
                     new AggItem
                     {
                          AggType= AggType.Sum,
                          NavigatePaths= new List<ValuePath>
                          {
                             new ValuePath{ Name= nameof(Entity<T>.Value)}
                          }
                     }
                 }
            });
            var sumMethod = typeof(Queryable).GetMethods()
                .Where(p => p.Name == nameof(Queryable.Sum))
                .Where(p => p.GetParameters().Length == 1)
                .Where(p => p.GetParameters().First().ParameterType.GetGenericArguments().First() == typeof(T))
                .Single();
            var expectedRes = sumMethod.Invoke(null, new object[] { source.Select(p => p.Value) });
            var actualRes = res.First().Value;
            testOutputHelper.WriteLine($"execute Sum, expected value is {expectedRes}, actual value is {actualRes}. ");
            actualRes.Should().Be(expectedRes);
        }

        private void ShouldAggValueInternal<T>(int caseCount, AggType aggType, string name)
            where T : struct
        {
            var source = GetTestSources<T>(caseCount);
            var res = source.DoAgg(new AggInfo
            {
                Items = new List<AggItem>
                 {
                     new AggItem
                     {
                          AggType= aggType,
                          NavigatePaths= new List<ValuePath>
                          {
                             new ValuePath{ Name= nameof(Entity<T>.Value)}
                          }
                     }
                 }
            });
            var aggMethod = typeof(Queryable).GetMethods()
                .Where(p => p.Name == name)
                .Where(p => p.GetParameters().Length == 1)
                .Single();
            var aggMethodInstance = aggMethod.MakeGenericMethod(typeof(Nullable<T>));
            var expectedRes = aggMethodInstance.Invoke(null, new object[] { source.Select(p => (Nullable<T>)p.Value) });
            var actualRes = res.First().Value;
            testOutputHelper.WriteLine($"execute {name}, expected value is {expectedRes}, actual value is {actualRes}. ");
            actualRes.Should().Be(expectedRes);
        }

        private void ShouldNullableAggValueInternal<T>(int caseCount, AggType aggType, string name)
        {
            var source = GetTestSources<T>(caseCount);
            var res = source.DoAgg(new AggInfo
            {
                Items = new List<AggItem>
                 {
                     new AggItem
                     {
                          AggType= aggType,
                          NavigatePaths= new List<ValuePath>
                          {
                             new ValuePath{ Name= nameof(Entity<T>.Value)}
                          }
                     }
                 }
            });
            var aggMethod = typeof(Queryable).GetMethods()
                .Where(p => p.Name == name)
                .Where(p => p.GetParameters().Length == 1)
                .Single();

            var aggMethodInstance = aggMethod.MakeGenericMethod(typeof(T));
            var expectedRes = aggMethodInstance.Invoke(null, new object[] { source.Select(p => p.Value) });
            var actualRes = res.First().Value;
            testOutputHelper.WriteLine($"execute {name}, expected value is {expectedRes}, actual value is {actualRes}. ");
            actualRes.Should().Be(expectedRes);
        }


        private void ShouldAvgValueInternal<T>(int caseCount, AggType aggType, string name)
         where T : struct
        {
            var source = GetTestSources<T>(caseCount);
            var res = source.DoAgg(new AggInfo
            {
                Items = new List<AggItem>
                 {
                     new AggItem
                     {
                          AggType= aggType,
                          NavigatePaths= new List<ValuePath>
                          {
                             new ValuePath{ Name= nameof(Entity<T>.Value)}
                          }
                     }
                 }
            });
            var aggMethod = typeof(Queryable).GetMethods()
                .Where(p => p.Name == name)
                .Where(p => p.GetParameters().Length == 1)
                 .Where(p => p.GetParameters().First().ParameterType.GetGenericArguments().First() == typeof(Nullable<T>))

                .Single();
            var expectedRes = aggMethod.Invoke(null, new object[] { source.Select(p => (Nullable<T>)p.Value) });
            var actualRes = res.First().Value;
            testOutputHelper.WriteLine($"execute {name}, expected value is {expectedRes}, actual value is {actualRes}. ");
            actualRes.Should().Be(expectedRes);
        }

        private void ShouldNullableAvgValueInternal<T>(int caseCount, AggType aggType, string name)
        {
            var source = GetTestSources<T>(caseCount);
            var res = source.DoAgg(new AggInfo
            {
                Items = new List<AggItem>
                 {
                     new AggItem
                     {
                          AggType= aggType,
                          NavigatePaths= new List<ValuePath>
                          {
                             new ValuePath{ Name= nameof(Entity<T>.Value)}
                          }
                     }
                 }
            });
            var aggMethod = typeof(Queryable).GetMethods()
                .Where(p => p.Name == name)
                .Where(p => p.GetParameters().Length == 1)
                 .Where(p => p.GetParameters().First().ParameterType.GetGenericArguments().First() == typeof(T))

                .Single();

            var expectedRes = aggMethod.Invoke(null, new object[] { source.Select(p => p.Value) });
            var actualRes = res.First().Value;
            testOutputHelper.WriteLine($"execute {name}, expected value is {expectedRes}, actual value is {actualRes}. ");
            actualRes.Should().Be(expectedRes);
        }


        private IQueryable<Entity<T>> GetTestSources<T>(int count = 10, bool hasNumberRule = true)
        {
            var valueType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
            var faker = new Bogus.Faker<Entity<T>>();
            if (hasNumberRule)
            {
                faker = faker.RuleFor(p => p.Value, f => Convert.ChangeType(f.Random.Int(100, 5000) / 1.0, valueType));
            }
            var data = faker.Generate(count);
            return data.AsQueryable();
        }
        private IQueryable<Entity<T>> GetRepeadTestSources<T>(int count = 10, bool hasNumberRule = true)
        {
            var valueType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
            var faker = new Bogus.Faker<Entity<T>>();
            if (hasNumberRule)
            {
                faker = faker.RuleFor(p => p.Value, f => Convert.ChangeType(f.Random.Int(100, 5000) / 1.0, valueType));
            }
            var data = faker.Generate(count);
            return data.Concat(data).AsQueryable();
        }
        public record Entity<T>
        {
            public T Value { get; set; }

        }
    }
}
