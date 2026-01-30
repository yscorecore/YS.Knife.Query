using System;
using YS.Knife.Query.Parser;

namespace YS.Knife.Query
{
    public static class ObjectExtensions
    {
        public static bool IsMatched<T>(this T source, FilterInfo filterInfo)
        {
            _ = filterInfo ?? throw new ArgumentNullException(nameof(filterInfo));
            var lambda = FilterExtensions.CreateFilterLambdaExpression<T>(filterInfo);
            var func = lambda.Compile();
            return func(source);
        }
        public static bool IsMatched<T>(this T source, string filter)
        {
            _ = filter ?? throw new ArgumentNullException(nameof(filter));
            var filterInfo = ParseFilter(filter);
            return IsMatched(source, filterInfo);
        }
        static FilterInfo ParseFilter(string filter)
        {
            try
            {
                return FilterInfo.Parse(filter);
            }
            catch (ParseException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ParseException("parse filter error.", ex);
            }
        }
    }
}
