using System.Collections.Generic;

namespace Buran.Core.MvcLibrary.Grid.Pager
{
	public class StaticPagedList<T> : BasePagedList<T>
	{
		public StaticPagedList(IEnumerable<T> subset, int index, int pageSize, int totalItemCount)
			: base(index, pageSize, totalItemCount)
		{
			Subset.AddRange(subset);
		}
	}
}