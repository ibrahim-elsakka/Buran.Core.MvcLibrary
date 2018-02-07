using Buran.Core.MvcLibrary.Resource;

namespace Buran.Core.MvcLibrary.Grid.Pager
{
    public class PagedListRenderOptions
    {
        public bool SimpleTag { get; set; }

        public PagedListRenderOptions()
        {
            DisplayLinkToFirstPage = true;
            DisplayLinkToLastPage = true;
            DisplayLinkToPreviousPage = true;
            DisplayLinkToNextPage = true;
            DisplayLinkToIndividualPages = true;
            DisplayPageCountAndCurrentLocation = false;
            LinkToFirstPageFormat = "<<";
            LinkToPreviousPageFormat = "<";
            LinkToIndividualPageFormat = "{0}";
            LinkToNextPageFormat = ">";
            LinkToLastPageFormat = ">>";
            PageSizeText = Text.PageSizeKeyword;
            PageSize = 10;
        }

        public bool DisplayLinkToFirstPage { get; set; }

        public bool DisplayLinkToLastPage { get; set; }

        public bool DisplayLinkToPreviousPage { get; set; }

        public bool DisplayLinkToNextPage { get; set; }

        public bool DisplayLinkToIndividualPages { get; set; }

        public bool DisplayPageCountAndCurrentLocation { get; set; }

        public bool DisplayItemSliceAndTotal { get; set; }

        public string LinkToFirstPageFormat { get; set; }

        public string LinkToPreviousPageFormat { get; set; }

        public string LinkToIndividualPageFormat { get; set; }

        public string LinkToNextPageFormat { get; set; }

        public string LinkToLastPageFormat { get; set; }

        public string PageCountAndCurrentLocationFormat { get; set; }

        public string ItemSliceAndTotalFormat { get; set; }

        public string PageSizeText { get; set; }
        public int PageSize { get; set; }

        public static PagedListRenderOptions Minimal
        {
            get
            {
                return new PagedListRenderOptions
                        {
                            DisplayLinkToFirstPage = false,
                            DisplayLinkToLastPage = false,
                            DisplayLinkToIndividualPages = false
                        };
            }
        }

        public static PagedListRenderOptions MinimalWithPageCountText
        {
            get
            {
                return new PagedListRenderOptions
                {
                    DisplayLinkToFirstPage = false,
                    DisplayLinkToLastPage = false,
                    DisplayLinkToIndividualPages = false,
                    DisplayPageCountAndCurrentLocation = true
                };
            }
        }

        public static PagedListRenderOptions MinimalWithItemCountText
        {
            get
            {
                return new PagedListRenderOptions
                {
                    DisplayLinkToFirstPage = false,
                    DisplayLinkToLastPage = false,
                    DisplayLinkToIndividualPages = false,
                    DisplayItemSliceAndTotal = true
                };
            }
        }

        public static PagedListRenderOptions PageNumbersOnly
        {
            get
            {
                return new PagedListRenderOptions
                {
                    DisplayLinkToFirstPage = false,
                    DisplayLinkToLastPage = false,
                    DisplayLinkToPreviousPage = false,
                    DisplayLinkToNextPage = false
                };
            }
        }
    }
}