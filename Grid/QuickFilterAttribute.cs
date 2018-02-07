using System;

namespace Buran.Core.MvcLibrary.Grid
{
    public class QuickFilterAttribute : Attribute
    {
        public Type Repository { get; set; }
        public string QueryName { get; set; }
        public string Url { get; set; }
        public string SearchFieldName { get; set; }

        public QuickFilterAttribute(Type repository, string queryName = "GetSelectList", string searchFieldName = null)
        {
            Repository = repository;
            QueryName = queryName;
            SearchFieldName = searchFieldName;
        }

        public QuickFilterAttribute(string url)
        {
            Url = url;
        }
    }
}
