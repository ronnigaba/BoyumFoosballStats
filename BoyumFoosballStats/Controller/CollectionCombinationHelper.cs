namespace BoyumFoosballStats.Controller
{
    public static class CollectionCombinationHelper
    {
        public static IEnumerable<IEnumerable<T>>
            GetUniqueCombinations<T>(IEnumerable<T> list, int length) where T : IComparable
        {
            if (length == 1) return list.Select(t => new T[] { t });
            return GetUniqueCombinations(list, length - 1)
                .SelectMany(t => list.Where(o => o.CompareTo(t.Last()) > 0),
                    (t1, t2) => t1.Concat(new T[] { t2 }));
        }
        
        //ToDo - GetAllCombinations
    }
}
