﻿using System.Collections.Generic;

namespace System.Linq
{
    public static class EnumerableExtenstions
    {
        public static bool HasDuplicate<T>(this IEnumerable<T> source)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            HashSet<T> items = new HashSet<T>();
            foreach (var item in source)
            {
                if (items.Contains(item))
                {
                    return true;
                }
                else
                {
                    items.Add(item);
                }
            }
            return false;
        }
        public static bool HasDuplicate<T, S>(this IEnumerable<T> source, Func<T, S> selector)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            _ = selector ?? throw new ArgumentNullException(nameof(selector));
            HashSet<S> items = new HashSet<S>();
            foreach (var item in source)
            {
                var key = selector(item);
                if (items.Contains(key))
                {
                    return true;
                }
                else
                {
                    items.Add(key);
                }
            }
            return false;
        }
    }
}
