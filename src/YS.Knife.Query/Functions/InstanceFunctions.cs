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
            Add<DateTime, DateTime>(nameof(DateTime.AddDays), p => p.AddDays(It.Arg<double>()));
            Add<string, string>(nameof(string.ToLower), p => p.ToLower());
            Add<string, string>(nameof(string.ToUpper), p => p.ToUpper());
            Add<string, string>(nameof(string.Substring), p => p.Substring(It.Arg<int>()));

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
