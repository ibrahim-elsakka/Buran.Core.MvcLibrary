using System.Collections.Generic;

namespace Buran.Core.MvcLibrary.Grid.Pager
{
	public static class PagedListExtensions
	{
		public static IPagedList<T> ToPagedList<T>(this IEnumerable<T> superset, int index, int pageSize)
		{
			return new PagedList<T>(superset, index, pageSize);
		}
	}
}