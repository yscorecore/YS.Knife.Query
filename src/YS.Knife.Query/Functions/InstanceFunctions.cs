using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using YS.Knife.Query.Functions.Specials;

namespace YS.Knife.Query.Functions
{
    public class InstanceFunctions
    {
        private readonly static Dictionary<string, List<FunctionDesc>> All = new Dictionary<string, List<FunctionDesc>>(StringComparer.InvariantCultureIgnoreCase);
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
            Add<string, int>(nameof(string.CompareTo), p => p.CompareTo(It.Arg<string>()));
            Add<string, bool>(nameof(string.StartsWith), p => p.StartsWith(It.Arg<string>()));
            Add<string, bool>(nameof(string.EndsWith), p => p.EndsWith(It.Arg<string>()));
            Add<string, bool>(nameof(string.Contains), p => p.Contains(It.Arg<string>()));
            Add<string, int>(nameof(string.Length), p => p.Length);
            //Add<string, string>("left", p => p.Substring(It.Arg<int>()));
            Add(nameof(Any), typeof(ICollection<>), new Any());
            Add(nameof(Count), typeof(ICollection<>), new Count());
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
            if (All.TryGetValue(name, out var desc))
            {
                desc.Add(new FunctionDesc(type, function));
            }
            else
            {
                All[name] = new List<FunctionDesc>() { new FunctionDesc(type, function) };
            }

        }
        public static bool Remove(Type type, string name)
        {
            if (All.TryGetValue(name, out var list))
            {
                return list.RemoveAll(p => p.IsMatch(type)) > 0;
            }
            return false;
        }
        public static bool Contains(Type type, string name)
        {
            if (All.TryGetValue(name, out var list))
            {
                return list.Any(p => p.IsMatch(type));
            }
            return false;
        }

        public static IFunction Get(Type type, string name)
        {
            if (All.TryGetValue(name, out var list))
            {
                return list.FirstOrDefault(p => p.IsMatch(type))?.Instance;
            }
            return null;
        }

        private class FunctionDesc
        {
            public FunctionDesc(Type type, IFunction instance)
            {
                Type = type;
                this.Instance = instance;
            }

            public Type Type { get; }
            public IFunction Instance { get; set; }

            public bool IsMatch(Type instanceType)
            {
                if (Type.IsAssignableFrom(instanceType))
                {
                    return true;
                }
                if (Type.IsGenericTypeDefinition && instanceType.IsGenericType)
                {
                    if (instanceType.GetGenericTypeDefinition() == Type)
                    {
                        return true;
                    }
                    else if (instanceType.GetInterfaces().Any(p => p.IsGenericType && p.GetGenericTypeDefinition() == Type))
                    {
                        return true;
                    }
                }
                return false;
            }
        }
    }
}
