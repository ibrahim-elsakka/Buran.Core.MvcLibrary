using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Buran.Core.MvcLibrary.Grid
{
    public class DataGridOptions
    {
        public enum LayoutTypes
        {
            Table,
            TableScroll
        }

        public const string GridJsFunction = "LoadGrid";
        public DataGridOptions()
        {
            LayoutType = LayoutTypes.Table;
            TableId = "";

            var popupEditorCss = "fancybox fancybox.iframe";

            CssTable = "dataGrid table table-striped table-bordered table-hover no-footer";
            EmptyData = "<div>EMPTY RESULT</div>";

            Sortable = true;
            SortDirKeyword = "sortdir";
            SortKeyword = "sort";
            SortDefaultDirection = "ASC";

            PagerAndShortAction = "ListView";
            PagerJsFunction = GridJsFunction;
            GridDiv = "divList";
            PagerKeyword = "pageindex";
            PageSizeKeyword = "pagesize";
            PagerEnabled = true;
            PagerSize = 20;

            ButtonDeleteEnabled = true;
            ButtonDeleteAction = "delete";
            ButtonDeleteCss = "btn btn-xs red text-delete";

            ButtonEditEnabled = true;
            ButtonEditAction = "edit";
            EditPopup = false;
            ButtonEditCss = "btn btn-xs yellow text-edit";
            ButtonEditPopupCss = ButtonEditCss + " " + popupEditorCss;

            ButtonInsertAction = "create";
            ButtonInsertEnabled = false;
            InsertPopup = false;
            ButtonInsertCss = "btn btn-xs text-create";
            ButtonInsertPopupCss = ButtonInsertCss + " " + popupEditorCss;

            ButtonColumnWidth = 75;

            Buttons = new List<HtmlString>();
            HeaderButtons = new List<HtmlString>();
            FilteringEnabled = true;

            Popup = "";
            PopupSize = new Size(700, 450);

            RowOtherButtons = new List<HtmlString>();
            RowOtherButtonsCss = "btn btn-xs";

            ButtonRefreshEnabled = true;
            ButtonRefreshCss = "btn btn-xs text-refresh";
        }

        public LayoutTypes LayoutType { get; set; }
        public string CssTable { get; set; }
        public string TableId { get; set; }

        public string PagerAndShortAction { get; set; }
        public string PagerJsFunction { get; set; }
        public string GridDiv { get; set; }

        public string PagerKeyword { get; set; }
        public string PageSizeKeyword { get; set; }
        public int PagerSize { get; set; }
        public bool PagerEnabled { get; set; }

        public bool Sortable { get; set; }
        public string SortKeyword { get; set; }
        public string SortDirKeyword { get; set; }
        public string SortDefaultFieldName { get; set; }
        public IEnumerable<string> SortDefaultFieldNames { get; set; }
        public string SortDefaultDirection { get; set; }

        public bool ButtonInsertEnabled { get; set; }
        public string ButtonInsertAction { get; set; }
        public string ButtonInsertCss { get; set; }
        public string ButtonInsertPopupCss { get; set; }
        public bool InsertPopup { get; set; }

        public bool ButtonRefreshEnabled { get; set; }
        public string ButtonRefreshCss { get; set; }

        public bool ButtonEditEnabled { get; set; }
        public string ButtonEditAction { get; set; }
        public string ButtonEditCss { get; set; }
        public bool EditPopup { get; set; }
        public string ButtonEditPopupCss { get; set; }

        public bool ButtonDeleteEnabled { get; set; }
        public string ButtonDeleteAction { get; set; }
        public string ButtonDeleteCss { get; set; }

        public bool ButtonImportEnabled { get; set; }
        public string ButtonImportAction { get; set; }
        public string ButtonImportCss { get; set; }
        public string ButtonImportText { get; set; }

        public int ButtonColumnWidth { get; set; }
        public List<HtmlString> Buttons { get; set; }
        public List<HtmlString> HeaderButtons { get; set; }

        public string KeyField { get; set; }
        public string TextField { get; set; }

        public bool FilteringEnabled { get; set; }

        public string EmptyData { get; set; }

        public string Popup { get; set; }
        public Size PopupSize { get; set; }

        public List<HtmlString> RowOtherButtons { get; set; }
        public string RowOtherButtonsCss { get; set; }

        public string RowFormatFunction { get; set; }
        public string ButtonEditShowFunction { get; set; }
        public string ButtonDeleteShowFunction { get; set; }
        public Type RowFormatClass { get; set; }
    }
}
