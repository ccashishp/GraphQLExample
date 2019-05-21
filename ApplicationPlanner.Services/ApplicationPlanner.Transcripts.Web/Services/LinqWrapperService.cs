using ApplicationPlanner.Transcripts.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApplicationPlanner.Transcripts.Web.Services
{
    public interface ILinqWrapperService
    {
        ItemsCountModel<T> GetLinqedList<T>(IEnumerable<T> completeList, Func<T, bool> predicate, string filterByProperty, string filterByValue, string orderBy, SortOrder sortOrder, int skip, int take);
    }

    public class LinqWrapperService : ILinqWrapperService
    {
        public ItemsCountModel<T> GetLinqedList<T>(IEnumerable<T> completeList, Func<T, bool> predicate, string filterByProperty, string filterByValue, string orderBy, SortOrder sortOrder, int skip, int take)
        {
            var result = new ItemsCountModel<T>();

            // 1. Search based on the query
            var matchQueryList = predicate == null
                ? completeList
                : GetMatchQueryList(completeList, predicate);

            // 2. Filter
            var filteredList = String.IsNullOrWhiteSpace(filterByProperty) || String.IsNullOrWhiteSpace(filterByValue)
                ? matchQueryList
                : GetFilteredList(matchQueryList, filterByProperty, filterByValue);

            // 3. Set count
            result.Count = filteredList.Count();

            // 4. Order/Sort
            var ordedList = String.IsNullOrWhiteSpace(orderBy) ? filteredList : GetSortedList(filteredList, orderBy, sortOrder);

            // 5. Skip
            var skipList = ordedList.Skip(skip);

            // 6. Take
            var takeList = take == int.MaxValue ? skipList : skipList.Take(take);

            result.Items = takeList;

            return result;
        }

        public IEnumerable<T> GetMatchQueryList<T>(IEnumerable<T> ListToSearch, Func<T, bool> predicate)
        {
            return ListToSearch.Where(predicate);
        }

        public IEnumerable<T> GetFilteredList<T>(IEnumerable<T> ListToFilter, string filterByProperty, string filterByValue)
        {
            filterByProperty = Char.ToUpperInvariant(filterByProperty[0]) + filterByProperty.Substring(1); // Make sure the property name is CamelCase
            var propertyInfo = typeof(T).GetProperty(filterByProperty);
            return ListToFilter.Where(x => propertyInfo.GetValue(x, null).ToString() == filterByValue);
        }

        public IEnumerable<T> GetSortedList<T>(IEnumerable<T> ListToSort, string orderBy, SortOrder sortOrder)
        {
            orderBy = Char.ToUpperInvariant(orderBy[0]) + orderBy.Substring(1); // Make sure the property name is CamelCase
            var propertyInfo = typeof(T).GetProperty(orderBy);
            // Sort Order: ASC or DESC
            return sortOrder == SortOrder.ASC
                ? ListToSort.OrderBy(x => propertyInfo.GetValue(x, null))
                : ListToSort.OrderByDescending(x => propertyInfo.GetValue(x, null));
        }
    }
}
