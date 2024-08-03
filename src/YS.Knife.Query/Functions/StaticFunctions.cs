using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace YS.Knife.Query.Functions
{
    public class StaticFunctions
    {
        private readonly static Dictionary<string, IFunction> All = new Dictionary<string, IFunction>(StringComparer.InvariantCultureIgnoreCase);
        static StaticFunctions()
        {
            Add(nameof(DateTime.Now), () => DateTime.Now);
            Add(nameof(Guid.NewGuid), () => Guid.NewGuid());
            Add(nameof(string.Join), () => string.Join(",", It.Arg<string[]>()));
        }

        public static void Add<T>(string name, Expression<Func<T>> body)
        {
            Add(name, new StaticFunction<T>(body));
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
