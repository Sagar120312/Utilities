using SearchUtility.DTOs;

namespace ListSearchSortFilterUtility.Operation.Interface
{
    public interface IListFilterOperation
    {
        List<T> ApplyFilters<T>(List<T> data, List<FilterCondition> filters = null, List<SortCondition> sortConditions = null, string search = null, int? top = null, int? skip = null);
    }
}
