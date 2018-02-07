using Buran.Core.MvcLibrary.Resource;
using Buran.Core.MvcLibrary.Utils;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Text;

namespace Buran.Core.MvcLibrary.Grid.Pager
{
    public static class HtmlHelper2
    {
        private static TagBuilder WrapInListItem(TagBuilder inner, params string[] classes)
        {
            var li = new TagBuilder("li");
            foreach (var @class in classes)
                li.AddCssClass(@class);
            li.InnerHtml.SetHtmlContent(inner.GetString());
            return li;
        }

        private static TagBuilder Result(IPagedList list)
        {
            var pageMax = ((list.PageIndex + 1) * list.PageSize);
            var pageMin = (list.PageIndex * list.PageSize);
            if (pageMax > list.TotalItemCount)
                pageMax = list.TotalItemCount;
            if (pageMin == 0)
                pageMin = list.TotalItemCount == 0 ? 0 : 1;
            var first = new TagBuilder("span");
            if (list.TotalItemCount == 0)
            {
                first.InnerHtml.SetHtmlContent(Text.PagerDesc_No);
            }
            else
            {
                first.InnerHtml.SetHtmlContent(string.Format(Text.PagerDesc,
                    pageMin,
                    pageMax,
                    list.TotalItemCount
                ));
            }
            return first;
        }

        private static TagBuilder First(IPagedList list, Func<int, string> generatePageUrl, string format)
        {
            const int targetPageIndex = 0;
            if (list.IsFirstPage)
                return null;
            var first = new TagBuilder("a");
            first.InnerHtml.SetHtmlContent(string.Format(format, targetPageIndex + 1));
            first.Attributes["href"] = generatePageUrl(targetPageIndex);
            return WrapInListItem(first, "first");
        }

        private static TagBuilder Previous(IPagedList list, Func<int, string> generatePageUrl, string format)
        {
            var targetPageIndex = list.PageIndex - 1;
            if (!list.HasPreviousPage)
                return null;
            var previous = new TagBuilder("a");
            previous.InnerHtml.SetHtmlContent(string.Format(format, targetPageIndex + 1));
            previous.Attributes["href"] = generatePageUrl(targetPageIndex);
            return WrapInListItem(previous, "previous");
        }

        private static TagBuilder Page(int i, IPagedList list, Func<int, string> generatePageUrl, string format)
        {
            var targetPageIndex = i;
            var page = new TagBuilder("a");
            page.InnerHtml.SetHtmlContent(string.Format(format, targetPageIndex + 1));
            var @class = "";
            if (i == list.PageIndex)
            {
                page.Attributes["href"] = "#";
                @class = "active";
            }
            else
                page.Attributes["href"] = generatePageUrl(targetPageIndex);
            return WrapInListItem(page, @class);
        }

        private static TagBuilder Next(IPagedList list, Func<int, string> generatePageUrl, string format)
        {
            var targetPageIndex = list.PageIndex + 1;
            if (!list.HasNextPage)
                return null;
            var next = new TagBuilder("a");
            next.InnerHtml.SetHtmlContent(string.Format(format, targetPageIndex + 1));
            next.Attributes["href"] = generatePageUrl(targetPageIndex);
            return WrapInListItem(next, "next");
        }

        private static TagBuilder Last(IPagedList list, Func<int, string> generatePageUrl, string format)
        {
            var targetPageIndex = list.PageCount - 1;
            if (list.IsLastPage)
                return null;
            var last = new TagBuilder("a");
            last.InnerHtml.SetHtmlContent(string.Format(format, targetPageIndex + 1));
            last.Attributes["href"] = generatePageUrl(targetPageIndex);
            return WrapInListItem(last, "last");
        }

        private static TagBuilder PageSizeCombo(string format, int currentPageSize, string pageSizeUrl)
        {
            var text = new TagBuilder("span");
            text.InnerHtml.SetHtmlContent(format);

            var select = new TagBuilder("select");
            select.Attributes["id"] = "ddPageSize";
            select.Attributes["data-url"] = pageSizeUrl;
            select.InnerHtml.SetHtmlContent($"<option value=\"{10}\" {(currentPageSize == 10 ? "selected=\"selected\"" : "")}>{10}</option>");
            select.InnerHtml.SetHtmlContent($"<option value=\"{20}\" {(currentPageSize == 20 ? "selected=\"selected\"" : "")}>{20}</option>");
            select.InnerHtml.SetHtmlContent($"<option value=\"{50}\" {(currentPageSize == 50 ? "selected=\"selected\"" : "")}>{50}</option>");
            select.InnerHtml.SetHtmlContent($"<option value=\"{100}\" {(currentPageSize == 100 ? "selected=\"selected\"" : "")}>{100}</option>");
            text.InnerHtml.SetHtmlContent(select);
            return text;
        }

        public static string PagedListPager2(this IHtmlHelper html, IPagedList list,
            Func<int, string> generatePageUrl, PagedListRenderOptions options, int currentPageSize, string pageSizeUrl)
        {
            var listItemLinks = new StringBuilder();

            var result = Result(list);

            if (options.DisplayLinkToFirstPage)
            {
                var first = First(list, generatePageUrl, options.LinkToFirstPageFormat);
                if (first != null)
                    listItemLinks.Append(first.GetString());
            }
            if (options.DisplayLinkToPreviousPage)
            {
                var pre = Previous(list, generatePageUrl, options.LinkToPreviousPageFormat);
                if (pre != null)
                    listItemLinks.Append(pre.GetString());
            }
            if (options.DisplayLinkToIndividualPages)
            {
                var max = 10;
                var half = (int)Math.Round((decimal)max / 2, 0, MidpointRounding.ToEven);
                var start = 0;
                var stop = max;
                if (list.PageIndex > half)
                {
                    start = list.PageIndex - half;
                    stop = start + max;
                }
                if (stop > list.PageCount)
                {
                    stop = list.PageCount;
                    start = stop - max;
                }
                if (start < 0)
                    start = 0;
                for (var i = start; i < stop; i++)
                    listItemLinks.Append(Page(i, list, generatePageUrl, options.LinkToIndividualPageFormat).GetString());
            }
            if (options.DisplayLinkToNextPage)
            {
                var next = Next(list, generatePageUrl, options.LinkToNextPageFormat);
                if (next != null)
                    listItemLinks.Append(next.GetString());
            }
            if (options.DisplayLinkToLastPage)
            {
                var last = Last(list, generatePageUrl, options.LinkToLastPageFormat);
                if (last != null)
                    listItemLinks.Append(last.GetString());
            }
            var pagerDiv = new TagBuilder("div");
            pagerDiv.AddCssClass("row");
            pagerDiv.GenerateId("divPager", "");

            var divLeft = new TagBuilder("div");
            divLeft.AddCssClass("col-md-3 col-sm-12");

            var divInfo = new TagBuilder("div");
            divInfo.AddCssClass("info");
            divInfo.InnerHtml.SetHtmlContent(result.GetString() + ", " + PageSizeCombo(options.PageSizeText, currentPageSize, pageSizeUrl).GetString());
            divLeft.InnerHtml.SetHtmlContent(divInfo.GetString());

            var divRight = new TagBuilder("div");
            divRight.AddCssClass("col-md-9 col-sm-12");

            var divPager = new TagBuilder("div");
            divPager.AddCssClass("paginate");

            var outerDiv = new TagBuilder("ul");
            outerDiv.AddCssClass("pagination pagination-sm");
            outerDiv.InnerHtml.SetHtmlContent(listItemLinks.ToString());
            divPager.InnerHtml.SetHtmlContent(outerDiv.GetString());
            divRight.InnerHtml.SetHtmlContent(divPager.GetString());

            pagerDiv.InnerHtml.SetHtmlContent(divLeft.GetString() + divRight.GetString());

            return pagerDiv.GetString();
        }

        public static string PagedListPager<T>(IPagedList<T> paged, Func<int, string> generatePageUrl, PagedListRenderOptions options, int index, int pagerSize)
        {
            if (paged.PageCount == 1)
                return "";

            var listItemLinks = new StringBuilder();
            if (options.DisplayLinkToFirstPage)
            {
                var first = First(paged, generatePageUrl, options.LinkToFirstPageFormat);
                if (first != null)
                    listItemLinks.Append(first);
            }
            if (options.DisplayLinkToPreviousPage)
            {
                var pre = Previous(paged, generatePageUrl, options.LinkToPreviousPageFormat);
                if (pre != null)
                    listItemLinks.Append(pre);
            }
            if (options.DisplayLinkToIndividualPages)
            {
                var max = pagerSize;
                var half = (int)Math.Round((decimal)max / 2, 0, MidpointRounding.ToEven);
                var start = 0;
                var stop = max;
                if (paged.PageIndex > half)
                {
                    start = paged.PageIndex - half;
                    stop = start + max;
                }
                if (stop > paged.PageCount)
                {
                    stop = paged.PageCount;
                    start = stop - max;
                }
                if (start < 0)
                    start = 0;
                for (var i = start; i < stop; i++)
                    listItemLinks.Append(Page(i, paged, generatePageUrl, options.LinkToIndividualPageFormat));
            }
            if (options.DisplayLinkToNextPage)
            {
                var next = Next(paged, generatePageUrl, options.LinkToNextPageFormat);
                if (next != null)
                    listItemLinks.Append(next);
            }
            if (options.DisplayLinkToLastPage)
            {
                var last = Last(paged, generatePageUrl, options.LinkToLastPageFormat);
                if (last != null)
                    listItemLinks.Append(last);
            }
            var outerDiv = new TagBuilder("div");
            outerDiv.AddCssClass("pagination pagination-sm");
            outerDiv.InnerHtml.SetHtmlContent("<ul>" + listItemLinks + "</ul>");
            return outerDiv.ToString();
        }
    }
}