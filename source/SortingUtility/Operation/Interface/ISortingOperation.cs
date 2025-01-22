namespace SortingUtility.Operation.Interface
{
    public interface ISortingOperation
    {
        List<T> SortList<T>(List<T> list, string key, string order, Comparison<T> comparison);
    }
}
