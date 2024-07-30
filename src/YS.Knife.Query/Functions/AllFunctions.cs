using System;
using System.Collections.Generic;
using System.Linq;


namespace YS.Knife.Query.Functions
{
    public class AllFunctions
    {
        static AllFunctions()
        {
            LoadCurrentAssemblyFunctions();
        }
        private readonly static Dictionary<string, IFunction> All = new Dictionary<string, IFunction>(StringComparer.InvariantCultureIgnoreCase);

        public static void Add(string name, IFunction function)
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

        private static void LoadCurrentAssemblyFunctions()
        {
            typeof(AllFunctions)
                .Assembly.GetTypes()
                .Where(p => typeof(IFunction).IsAssignableFrom(p) && !p.IsAbstract)
                .Select(p => new KeyValuePair<string, IFunction>(p.Name, (IFunction)Activator.CreateInstance(p)))
                .ToList()
                .ForEach(p =>
                {
                    All[p.Key] = p.Value;
                });
        }
    }
}
