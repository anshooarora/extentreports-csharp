using System.Collections.Generic;
using System.Linq;

namespace AventStack.ExtentReports.Utils
{
    public static class ListUtil
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
            {
                return true;
            }

            var collection = enumerable as ICollection<T>;
            if (collection != null)
                return collection.Count < 1;

            return !enumerable.Any();
        }
    }
}
