using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace YS.Knife.Query.Functions
{
    public class InstanceFunctions
    {
        private readonly static Dictionary<string, IFunction> All = new Dictionary<string, IFunction>(StringComparer.InvariantCultureIgnoreCase);
        static InstanceFunctions()
        {
            Add<DateTime, DateTime>(nameof(DateTime.AddYears), p => p.AddYears(It.Arg<int>()));
            Add<DateTime, DateTime>(nameof(DateTime.AddMonths), p => p.AddMonths(It.Arg<int>()));
            Add<DateTime, DateTime>(nameof(DateTime.AddDays), p => p.AddDays(It.Arg<double>()));
            Add<DateTime, DateTime>(nameof(DateTime.AddHours), p => p.AddHours(It.Arg<double>()));
            Add<DateTime, DateTime>(nameof(DateTime.AddMinutes), p => p.AddMinutes(It.Arg<double>()));
            Add<DateTime, DateTime>(nameof(DateTime.AddSeconds), p => p.AddSeconds(It.Arg<double>()));
            Add<DateTime, DateTime>(nameof(DateTime.AddMilliseconds), p => p.AddMilliseconds(It.Arg<double>()));
            Add<DateTime, int>(nameof(DateTime.Year), p => p.Year);
            Add<DateTime, int>(nameof(DateTime.Month), p => p.Month);
            Add<DateTime, int>(nameof(DateTime.Day), p => p.Day);
            Add<DateTime, int>(nameof(DateTime.Hour), p => p.Hour);
            Add<DateTime, int>(nameof(DateTime.Minute), p => p.Minute);
            Add<DateTime, int>(nameof(DateTime.Second), p => p.Second);
            Add<DateTime, int>(nameof(DateTime.Millisecond), p => p.Millisecond);
            Add<DateTime, DayOfWeek>(nameof(DateTime.DayOfWeek), p => p.DayOfWeek);
            Add<DateTime, DateTime>(nameof(DateTime.Date), p => p.Date);

            Add<string, string>(nameof(string.ToLower), p => p.ToLower());
            Add<string, string>(nameof(string.ToUpper), p => p.ToUpper());
            Add<string, int>(nameof(string.Length), p => p.Length);
            //Add<string, string>("left", p => p.Substring(It.Arg<int>()));

        }

        public static void Add<Source, Target>(string name, Expression<Func<Source, Target>> body)
        {
            Add(name, typeof(Source), new InstanceFunction<Source, Target>(body));
        }
        private static void Add(string name, Type type, IFunction function)
        {
            _ = function ?? throw new ArgumentNullException(nameof(function));
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
            All[name] = function;
        }
        public static bool Remove(Type type, string name)
        {
            return All.Remove(name);
        }

        public static IFunction Get(Type type, string name)
        {
            return All.TryGetValue(name, out var fun) ? fun : null;
        }
    }
}
