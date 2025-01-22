using SortingUtility.Operation.Interface;
using System.Data;
using System.Reflection;

namespace SortingUtility.Operation.Abstract
{
    public class SortingOperation : ISortingOperation
    {
        /// <summary>
        /// Sorts a given list of any type in ascending or descending order using a custom key or comparison.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">The list to sort.</param>
        /// <param name="key">Optional: The name of the property to sort by (for object lists).</param>
        /// <param name="order">The order of sorting: "asc" for ascending (default) or "desc" for descending.</param>
        /// <param name="comparison">Optional: A custom comparison function.</param>
        /// <returns>A sorted list based on the provided parameters.</returns>
        public List<T> SortList<T>(List<T> list, string key = null, string order = "asc", Comparison<T>? comparison = null)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (order != "asc" && order != "desc") throw new ArgumentException("Order must be 'asc' or 'desc'.", nameof(order));

            var sortedList = new List<T>(list); // Create a copy to avoid modifying the original list.

            // If a key is provided, sort by the key using reflection.
            if (!string.IsNullOrEmpty(key))
            {
                var type = typeof(T);
                var property = type.GetProperty(key, BindingFlags.Public | BindingFlags.Instance);

                if (property == null)
                {
                    throw new ArgumentException($"Property '{key}' does not exist on type '{type.Name}'.");
                }

                if (!typeof(IComparable).IsAssignableFrom(property.PropertyType))
                {
                    throw new InvalidOperationException($"Property '{key}' does not implement IComparable.");
                }

                // Sort using property values and order.
                sortedList = order == "asc"
                    ? sortedList.OrderBy(item => property.GetValue(item)).ToList()
                    : sortedList.OrderByDescending(item => property.GetValue(item)).ToList();
            }
            else if (comparison != null)
            {
                // Use custom comparison if provided.
                sortedList.Sort(comparison);

                if (order == "desc")
                {
                    sortedList.Reverse();
                }
            }
            else if (typeof(IComparable<T>).IsAssignableFrom(typeof(T)) || typeof(IComparable).IsAssignableFrom(typeof(T)))
            {
                // Use default comparer and order.
                sortedList.Sort();

                if (order == "desc")
                {
                    sortedList.Reverse();
                }
            }
            else
            {
                throw new InvalidOperationException($"Type {typeof(T).Name} does not implement IComparable.");
            }

            return sortedList;
        }
    }

}
