using System.Collections.Generic;

namespace Buran.Core.MvcLibrary.Grid.Pager
{
	public interface IPagedList<T> : IPagedList, IEnumerable<T>
	{
		T this[int index] { get; }
		int Count { get; }
	}

	public interface IPagedList
	{
		int PageCount { get; }

		int TotalItemCount { get; }

		int PageIndex { get; }

		int PageNumber { get; }

		int PageSize { get; }

		bool HasPreviousPage { get; }

		bool HasNextPage { get; }

		bool IsFirstPage { get; }

		bool IsLastPage { get; }

		int FirstItemOnPage { get; }

		int LastItemOnPage { get; }
	}
}