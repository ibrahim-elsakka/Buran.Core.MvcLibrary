using System;
using System.Collections;
using System.Collections.Generic;

namespace Buran.Core.MvcLibrary.Grid.Pager
{
    public abstract class BasePagedList<T> : IPagedList<T>
    {
        protected List<T> Subset = new List<T>();

        protected internal BasePagedList(int index, int pageSize, int totalItemCount)
        {
            TotalItemCount = totalItemCount;
            PageSize = pageSize;
            if (pageSize == -1)
                PageSize = totalItemCount;
            PageIndex = index;
            if (TotalItemCount > 0)
                PageCount = (int)Math.Ceiling(TotalItemCount / (double)PageSize);
            else
                PageCount = 0;

            if (PageCount > 0 && PageIndex >= PageCount)
                PageIndex = PageCount - 1;

            if (PageIndex < 0)
                throw new ArgumentOutOfRangeException("index", index, "PageIndex cannot be below 0.");
            if (PageSize < 1)
                throw new ArgumentOutOfRangeException("pageSize", pageSize, "PageSize cannot be less than 1.");
        }

        #region IPagedList<T> Members

        public int PageCount { get; protected set; }

        public int TotalItemCount { get; protected set; }

        public int PageIndex { get; protected set; }

        public int PageNumber
        {
            get { return PageIndex + 1; }
        }

        public int PageSize { get; protected set; }

        public bool HasPreviousPage
        {
            get { return PageIndex > 0; }
        }

        public bool HasNextPage
        {
            get { return PageIndex < (PageCount - 1); }
        }

        public bool IsFirstPage
        {
            get { return PageIndex <= 0; }
        }

        public bool IsLastPage
        {
            get { return PageIndex >= (PageCount - 1); }
        }

        public int FirstItemOnPage
        {
            get { return (PageIndex * PageSize) + 1; }
        }

        public int LastItemOnPage
        {
            get
            {
                var numberOfLastItemOnPage = FirstItemOnPage + PageSize - 1;
                if (numberOfLastItemOnPage > TotalItemCount)
                    numberOfLastItemOnPage = TotalItemCount;
                return numberOfLastItemOnPage;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Subset.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public T this[int index]
        {
            get { return Subset[index]; }
        }

        public int Count
        {
            get { return Subset.Count; }
        }

        #endregion
    }
}