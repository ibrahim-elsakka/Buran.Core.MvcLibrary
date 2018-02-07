using System.Collections.Generic;
using System.Linq;

namespace Buran.Core.MvcLibrary.Grid.Pager
{
    public class PagedList<T> : BasePagedList<T>
    {
        public PagedList(IEnumerable<T> superset, int index, int pageSize)
            : this(superset == null ? new List<T>().AsQueryable() : superset.AsQueryable(), index, pageSize)
        {
        }

        private PagedList(IQueryable<T> superset, int index, int pageSize)
            : base(index, pageSize, superset.Count())
        {
            if (TotalItemCount > 0)
            {
                if (pageSize == -1)
                    pageSize = TotalItemCount;
                if (index >= PageCount)
                    index = PageCount - 1;
                Subset = index == 0
                                    ? superset.Take(pageSize).ToList()
                                    : superset.Skip((index) * pageSize).Take(pageSize).ToList();
            }
        }
    }
}