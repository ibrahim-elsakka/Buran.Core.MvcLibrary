using Buran.Core.Library.Utils;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Collections.Generic;
using System.Linq;

namespace Buran.Core.MvcLibrary.Grid.Helper
{
    class SorterInfo
    {
        public string Keyword { get; set; }
        public string Direction { get; set; }
    }

    class Sorter
    {
        public List<SorterInfo> List { get; set; }
        public string CleanQueryString { get; set; }
        private UrlHelper urlHelper { get; set; }

        public Sorter(List<KeyValuePair<string, string>> query, string sortKeyword, string pagerAndShortAction, IHtmlHelper helper,
            string cleanQueryString)
        {
            CleanQueryString = cleanQueryString;
            urlHelper = new UrlHelper(helper.ViewContext);
            List = new List<SorterInfo>();

            var sortValueItem = query.FirstOrDefault(d => d.Key == sortKeyword);
            if (!sortValueItem.Value.IsEmpty())
            {
                var sortFields = sortValueItem.Value.Split(',');
                foreach (var field in sortFields)
                {
                    var s = field.Split(' ');
                    if (s.Count() == 2)
                    {
                        List.Add(new SorterInfo { Direction = s[1], Keyword = s[0] });
                    }
                }
            }
        }

        public string GetSortImg(string fieldName)
        {
            var sortingData = List.Where(d => d.Keyword == fieldName);
            if (sortingData.Count() > 0)
            {
                var sortInfo = sortingData.First();
                return sortInfo.Direction == "ASC"
                    ? "<img class='tableImg' src='" + urlHelper.Content(@"~/Content/admin/plugins/MvcGrid/images/sort-asc.png") + "' />"
                    : sortInfo.Direction == "DESC"
                            ? "<img class='tableImg' src='" + urlHelper.Content(@"~/Content/admin/plugins/MvcGrid/images/sort-desc.png") + "' />"
                            : string.Empty;
            }
            return string.Empty;
        }

        public string GetSortParam(string fieldName)
        {
            var str = string.Empty;
            var exits = false;
            foreach (var sortInfo in List)
            {
                if (!str.IsEmpty())
                    str += ",";
                if (sortInfo.Keyword == fieldName)
                {
                    exits = true;
                    str += sortInfo.Direction == "ASC"
                           ? fieldName + " DESC"
                           : sortInfo.Direction == "DESC"
                                 ? string.Empty
                                 : fieldName + " ASC";
                }
                else
                {
                    str += sortInfo.Keyword + " " + sortInfo.Direction;
                }
            }
            if (!exits)
            {
                if (!string.IsNullOrWhiteSpace(str))
                    str += ",";
                str += fieldName + " ASC";
            }
            return str;
        }
    }
}
