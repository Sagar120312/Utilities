using ListSearchSortFilterUtility.Operation.Interface;
using SearchUtility.DTOs;
using System.Data;
using System.Linq.Expressions;

namespace ListSearchSortFilterUtility.Operation.Abstract
{
    public class ListFilterOperation : IListFilterOperation
    {
        public List<T> ApplyFilters<T>(List<T> data, List<FilterCondition> filters = null, List<SortCondition> sortConditions = null, string search = null, int? top = null, int? skip = null)
        {
            // Apply filters if provided
            if (filters != null && filters.Any())
            {
                foreach (var filter in filters)
                {
                    data = ApplyFilter(data, filter);
                }
            }

            // Apply search if provided
            if (!string.IsNullOrEmpty(search))
            {
                data = ApplySearch(data, search);
            }

            // Apply sorting if provided
            if (sortConditions != null && sortConditions.Any())
            {
                data = ApplySorting(data, sortConditions);
            }

            // Apply pagination if provided
            if (skip.HasValue)
            {
                data = data.Skip(skip.Value).ToList();
            }

            if (top.HasValue)
            {
                data = data.Take(top.Value).ToList();
            }

            return data;
        }

        // Apply individual filter to the list of objects
        private List<T> ApplyFilter<T>(List<T> data, FilterCondition filter)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, filter.Field);
            var constant = Expression.Constant(Convert.ChangeType(filter.Value, property.Type));
            Expression comparison = null;

            // Create the comparison expression based on the operator
            switch (filter.Operator.ToLower())
            {
                case "eq":
                    comparison = Expression.Equal(property, constant);
                    break;
                case "gt":
                    comparison = Expression.GreaterThan(property, constant);
                    break;
                case "lt":
                    comparison = Expression.LessThan(property, constant);
                    break;
                case "ge":
                    comparison = Expression.GreaterThanOrEqual(property, constant);
                    break;
                case "le":
                    comparison = Expression.LessThanOrEqual(property, constant);
                    break;
                case "contains":
                    var method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                    comparison = Expression.Call(property, method, constant);
                    break;
                default:
                    throw new ArgumentException($"Operator {filter.Operator} not supported.");
            }

            var lambda = Expression.Lambda<Func<T, bool>>(comparison, parameter);
            var compiledLambda = lambda.Compile();
            return data.Where(compiledLambda).ToList();
        }

        // Apply search to check if any property contains the search term
        private List<T> ApplySearch<T>(List<T> data, string search)
        {
            return data.Where(item =>
            {
                var properties = item.GetType().GetProperties();
                foreach (var property in properties)
                {
                    var value = property.GetValue(item)?.ToString();
                    if (value != null && value.Contains(search, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
                return false;
            }).ToList();
        }

        // Apply sorting based on provided conditions
        private List<T> ApplySorting<T>(List<T> data, List<SortCondition> sortConditions)
        {
            IOrderedEnumerable<T> sortedData = null;

            foreach (var sort in sortConditions)
            {
                var parameter = Expression.Parameter(typeof(T), "x");
                var property = Expression.Property(parameter, sort.Field);
                var lambda = Expression.Lambda(property, parameter);

                var methodName = sort.Order.ToLower() == "desc" ? "OrderByDescending" : "OrderBy";
                var method = typeof(Enumerable).GetMethods()
                    .First(m => m.Name == methodName && m.GetParameters().Length == 2);

                var genericMethod = method.MakeGenericMethod(typeof(T), property.Type);
                sortedData = (IOrderedEnumerable<T>)genericMethod.Invoke(null, new object[] { data, lambda.Compile() });

                // Apply further sorting if required
                data = sortedData.ToList();
            }

            return sortedData?.ToList() ?? data;
        }
    }

}
