using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace YS.Knife.Query.Functions
{
    public class StaticFunctions
    {
        private readonly static Dictionary<string, IFunction> All = new Dictionary<string, IFunction>(StringComparer.InvariantCultureIgnoreCase);
        static StaticFunctions()
        {
            Add(nameof(DateTime.Now), () => DateTime.Now);
            Add(nameof(DateTime.UtcNow), () => DateTime.UtcNow);
            Add(nameof(DateTime.Today), () => DateTime.Today);
            Add(nameof(Guid.NewGuid), () => Guid.NewGuid());
            Add(nameof(string.Join), () => string.Join(",", It.Arg<string[]>()));
            Add(nameof(string.IsNullOrEmpty), () => string.IsNullOrEmpty(It.Arg<string>()));
            AddConstant("LatestWeek", () => GetThisWeek(DayOfWeek.Sunday));
            AddConstant("LatestWeek1", () => GetThisWeek(DayOfWeek.Monday));
            AddConstant("LatestWeek2", () => GetThisWeek(DayOfWeek.Tuesday));
            AddConstant("LatestWeek3", () => GetThisWeek(DayOfWeek.Wednesday));
            AddConstant("LatestWeek4", () => GetThisWeek(DayOfWeek.Thursday));
            AddConstant("LatestWeek5", () => GetThisWeek(DayOfWeek.Friday));
            AddConstant("LatestWeek6", () => GetThisWeek(DayOfWeek.Saturday));
            AddConstant("ThisMonth", () => new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1));
            AddConstant("ThisYear", () => new DateTime(DateTime.Today.Year, 1, 1));
        }
        private static DateTime GetThisWeek(DayOfWeek dayOfWeek)
        {
            var date = DateTime.Today;
            while (date.DayOfWeek != dayOfWeek)
            {
                date = date.AddDays(-1);
            }
            return date;
        }

        public static void Add<T>(string name, Expression<Func<T>> body)
        {
            Add(name, new StaticFunction<T>(body));
        }
        public static void AddConstant<T>(string name, Func<T> body)
        {
            var val = body();
            Add(name, () => val);
        }
        private static void Add(string name, IFunction function)
        {
            _ = function ?? throw new ArgumentNullException(nameof(function));
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
            All[name] = function;
        }
        public static bool Remove(string name)
        {
            return All.Remove(name);
        }

        public static IFunction Get(string name)
        {
            return All.TryGetValue(name, out var fun) ? fun : null;
        }
    }
}
