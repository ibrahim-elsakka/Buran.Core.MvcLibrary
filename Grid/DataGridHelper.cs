using System;
using System.Collections.Generic;
using System.Linq;
using Buran.Core.MvcLibrary.Grid.Pager;
using Buran.Core.MvcLibrary.Grid.Helper;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Html;
using Buran.Core.MvcLibrary.Grid.Columns;
using Buran.Core.Library.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Buran.Core.MvcLibrary.Resource;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using Buran.Core.Library.Utils;
using Microsoft.AspNetCore.Http.Extensions;
using System.Linq.Dynamic.Core;
using Buran.Core.MvcLibrary.Utils;

namespace Buran.Core.MvcLibrary.Grid
{
    public static class DataGridHelper
    {
        public static UrlHelper urlHelper;

        public static int TotalRowCount { get; private set; }
        public static int TotalPageCount { get; private set; }

        private static int _colCount;
        private static QueryString _query;
        private static Dictionary<string, StringValues> _queryDictionary;
        private static List<KeyValuePair<string, string>> _queryItems;
        private static string _queryParams = "";

        private static Sorter _sorter;
        private static Filter _filter;
        private static string _exportMode = string.Empty;

        private static string RefreshUrl(IHtmlHelper helper, DataGridOptions option)
        {
            var fs = urlHelper.Content(string.Format(@"~/{0}/{1}{2}",
                    LibGeneral.GetContentUrl(helper.ViewContext.RouteData),
                    option.PagerAndShortAction,
                    _queryParams));
            var refreshUrl = $"{option.PagerJsFunction}(\"{fs}\",\"{option.GridDiv}\");";
            return refreshUrl;
        }

        public static HtmlString DataGrid<T>(this IHtmlHelper helper, IEnumerable<T> data, DataColumn[] columns,
            DataGridOptions option)
            where T : class
        {
            urlHelper = new UrlHelper(helper.ViewContext);

            _exportMode = "";
            _query = helper.ViewContext.HttpContext.Request.QueryString;
            _queryDictionary = QueryHelpers.ParseQuery(_query.ToString());
            _queryItems = _queryDictionary.SelectMany(x => x.Value, (col, value) => new KeyValuePair<string, string>(col.Key, value)).ToList();
            _queryParams = _query.ToUriComponent();
            if (!option.PagerAndShortAction.IsEmpty())
            {
                var psss = option.PagerAndShortAction.Split('?');
                if (psss.Length > 1)
                    _queryParams = _queryParams.Replace("?" + psss[1], "");
            }

            #region SELECTABLE
            if (string.IsNullOrWhiteSpace(_exportMode))
            {
#if (DEBUG)
                if (string.IsNullOrWhiteSpace(option.KeyField))
                    throw new Exception("KEYFIELD not found");
#endif
            }
            #endregion
            _colCount = 0;
            var items = data;
            if (items == null)
            {
                var content = new HtmlContentBuilder().AppendHtml(option.EmptyData);
                return new HtmlString(content.GetString());
            }

            _sorter = new Sorter(_queryItems, option.SortKeyword, option.PagerAndShortAction, helper);
            _filter = new Filter(_queryDictionary, option.PagerKeyword,
                helper.ViewContext.RouteData, option.PagerAndShortAction,
                helper, option.PagerJsFunction, option.GridDiv);

            #region SORT DATA
            if (option.Sortable && !(data is PagedList<T>) && !(data is StaticPagedList<T>))
            {
                if (_sorter.List.Count == 0)
                {
                    if (!string.IsNullOrWhiteSpace(option.SortDefaultFieldName))
                        _sorter.List.Add(new SorterInfo() { Direction = option.SortDefaultDirection, Keyword = option.SortDefaultFieldName });
                    if (option.SortDefaultFieldNames != null)
                    {
                        foreach (string sortinfodefaultname in option.SortDefaultFieldNames)
                        {
                            _sorter.List.Add(new SorterInfo()
                            {
                                Direction = option.SortDefaultDirection,
                                Keyword = sortinfodefaultname
                            });
                        }
                    }
                }
                var sorting = string.Empty;
                foreach (var info in _sorter.List)
                {
                    if (!string.IsNullOrWhiteSpace(sorting))
                        sorting += ",";
                    sorting += String.Format("{0} {1}", info.Keyword, info.Direction);
                }
                items = items.AsQueryable().OrderBy(!string.IsNullOrWhiteSpace(sorting)
                    ? sorting
                    : String.Format("{0} {1}", option.KeyField, (string.IsNullOrWhiteSpace(option.SortDefaultDirection)
                        ? "ASC"
                        : option.SortDefaultDirection)));
            }
            #endregion

            #region DO PAGING
            var currentPageSize = option.ItemListCountList.Count > 0
                    ? option.ItemListCountList.First()
                    : 20;
            var currentPageIndexItem = _queryItems.FirstOrDefault(d => d.Key == option.PagerKeyword);
            var pageIndex = 0;
            if (!currentPageIndexItem.Value.IsEmpty())
            {
                int.TryParse(currentPageIndexItem.Value, out pageIndex);
            }
            if (string.IsNullOrWhiteSpace(_exportMode) && option.PagerEnabled && !(data is PagedList<T>) && !(data is StaticPagedList<T>))
            {
                var currentPageSizeStr = _queryItems.FirstOrDefault(d => d.Key == option.PageSizeKeyword);
                if (!currentPageSizeStr.Value.IsEmpty())
                {
                    int.TryParse(currentPageSizeStr.Value, out currentPageSize);
                }
                //option.PagerSize = currentPageSize;
                items = new PagedList<T>(items, pageIndex, currentPageSize);
                TotalRowCount = (items as PagedList<T>).TotalItemCount;
                TotalPageCount = (items as PagedList<T>).PageCount;
            }
            #endregion

            T firstItem = default(T);
            if (items != null && items.Any())
                firstItem = items.First();
            Type firstItemType = null;
            if (firstItem != null)
                firstItemType = firstItem.GetType();

            var writer = new HtmlContentBuilder();
            var tableScrollFirst = option.LayoutType == DataGridOptions.LayoutTypes.TableScroll && pageIndex == 0;
            var tableScroll = option.LayoutType == DataGridOptions.LayoutTypes.TableScroll && pageIndex > 0;

            if (option.LayoutType == DataGridOptions.LayoutTypes.Table || tableScrollFirst)
            {
                var refreshUrl = RefreshUrl(helper, option);
                writer.AppendHtml($"<div id='{"dataGrid-" + option.GridDiv}' data-refreshurl='{refreshUrl}' class='{"table-scrollable"}'>");
                writer.AppendHtml($"<table id='{option.TableId}' class='{option.CssTable}'>");
            }
            if (option.LayoutType == DataGridOptions.LayoutTypes.Table || tableScrollFirst)
            {
                writer.AppendHtml("<thead>");
                _colCount = columns.Count(d => d.Visible);
                if (option.ButtonDeleteEnabled || option.ButtonEditEnabled || option.ButtonRefreshEnabled)
                    _colCount++;
                if (string.IsNullOrWhiteSpace(_exportMode))
                    writer.AppendHtml(RenderHeaderBar(option));
                if (string.IsNullOrWhiteSpace(_exportMode) && option.FilteringEnabled)
                    writer.AppendHtml(RenderFilterRow(columns, firstItemType));
                writer.AppendHtml(RenderHeader<T>(helper, columns, option, firstItemType));
                writer.AppendHtml("</thead>");
            }
            if (option.LayoutType == DataGridOptions.LayoutTypes.Table || tableScrollFirst)
                writer.AppendHtml("<tbody>");
            foreach (var item in items)
                writer.AppendHtml(RenderRow(helper, columns, item, option));
            if (tableScrollFirst || tableScroll)
                writer.AppendHtml(RenderNextScrollLink(helper, option, pageIndex));
            if (option.LayoutType == DataGridOptions.LayoutTypes.Table || tableScrollFirst)
            {
                writer.AppendHtml("</thead>");
                writer.AppendHtml("</table>");
                writer.AppendHtml("</div>");
            }

            if (_exportMode.IsEmpty() && option.FilteringEnabled
                && columns.Count(d => !d.FilterValue.IsEmpty()) > 0)
            {
                writer.AppendHtml(RenderFilterRowFooter(columns, option));
            }
            if (_exportMode.IsEmpty() && option.PagerEnabled
                && option.LayoutType != DataGridOptions.LayoutTypes.TableScroll)
            {
                if (data is PagedList<T> || data is StaticPagedList<T>)
                {
                    var pager = RenderPager(helper, data, option, currentPageSize);
                    writer.AppendHtml(pager);
                }
                else
                {
                    var pager = RenderPager(helper, items, option, currentPageSize);
                    writer.AppendHtml(pager);
                }
            }
            writer.AppendHtml(RenderScript(helper, option));
            return new HtmlString(writer.GetString());
        }

        private static string RenderHeaderBar(DataGridOptions option)
        {
            var builder = new HtmlContentBuilder();
            var printHeader = option.HeaderButtons.Count > 0;
            if (!printHeader)
                return string.Empty;

            builder.AppendHtml($"<tr><th class='actions-header' colspan='{_colCount}'>");
            builder.AppendHtml("<div class='actions pull-right'><ul>");
            if (option.HeaderButtons.Count > 0)
            {
                foreach (var button in option.HeaderButtons)
                    builder.AppendHtml($"<li>{button.ToString()}</li>");
            }
            builder.AppendHtml("</ul></div></th></tr>");
            return builder.GetString();
        }

        private static string RenderHeader<T>(IHtmlHelper helper, IEnumerable<DataColumn> columns, DataGridOptions option, Type firstItemType)
        {
            var builder = new HtmlContentBuilder();
            builder.AppendHtml("<tr>");
            var urlOperator = option.PagerAndShortAction.IndexOf("?") > -1 ? "&" : "?";

            if (string.IsNullOrWhiteSpace(_exportMode) && (option.ButtonDeleteEnabled || option.ButtonEditEnabled
                || option.Buttons.Count > 0 || option.ButtonRefreshEnabled))
            {
                builder.AppendHtml($"<th width='{option.ButtonColumnWidth}'>");
                builder.AppendHtml("<div class='btn-group'>");
                if (option.ButtonInsertEnabled)
                {
                    var action = option.ButtonInsertAction;
                    if (option.GridDiv != "divList")
                    {
                        if (action.Contains("?"))
                            action += "&";
                        else
                            action += "?";
                        action += "grid=" + option.GridDiv;
                    }
                    var buttonClass = option.InsertPopup ? option.ButtonInsertPopupCss : option.ButtonInsertCss;
                    var pop = option.InsertPopup ? option.Popup : "";
                    builder.AppendHtml(string.Format("<a href='{0}/{1}' class='{2}' {4}>{3}</a>",
                        urlHelper.Content($"~/{LibGeneral.GetContentUrl(helper.ViewContext.RouteData)}"),
                        action,
                        buttonClass,
                        "<span class=\"fa fa-plus\"></span>",
                        pop));
                }
                if (option.ButtonRefreshEnabled)
                {
                    var refreshUrl = RefreshUrl(helper, option);
                    builder.AppendHtml(string.Format("<a onClick='{0}' id='btnGridRefresh-{3}' class='{1}'>{2}</a>",
                       refreshUrl,
                       option.ButtonRefreshCss,
                       "<span class=\"fa fa-refresh\"></span>",
                       option.GridDiv));
                }
                builder.AppendHtml("</div>");
                builder.AppendHtml("</th>");
            }
            foreach (var field in columns.Where(d => d.Visible))
            {
                var headClass = field.HeaderCssClass.IsEmpty() ? "" : $"class=\"{field.HeaderCssClass}\"";
                if (field.Width > 0)
                    builder.AppendHtml($"<th width='{field.Width}' {headClass}>");
                else
                    builder.AppendHtml($"<th {headClass}>");
                if (field.DataColumnType == DataColumnTypes.BoundColumn)
                {
                    if (string.IsNullOrWhiteSpace(field.Caption))
                        field.Caption = Digger.GetDisplayName(typeof(T), field.FieldName);
                    builder.AppendHtml("<div class='tableDiv'>");
                    if (string.IsNullOrWhiteSpace(_exportMode) && option.Sortable && field.Sortable)
                    {
                        var sortFieldName = field.SortField.IsEmpty() ? field.FieldName : field.SortField;
                        var sortImg = _sorter.GetSortImg(sortFieldName);
                        var sortParam = _sorter.GetSortParam(sortFieldName);

                        var url = urlHelper.Content(string.Format(@"~/{0}/{1}{5}{2}={3}&{4}",
                               LibGeneral.GetContentUrl(helper.ViewContext.RouteData),
                               option.PagerAndShortAction,
                               option.SortKeyword,
                               sortParam,
                               _sorter.CleanQueryString,
                               urlOperator));

                        builder.AppendHtml($"<a class='textHeader' href=\"javascript:{option.PagerJsFunction}('{url}', '{option.GridDiv}');\">{field.Caption}</a>");
                        builder.AppendHtml(sortImg);
                    }
                    else
                        builder.AppendHtml($"<span class='textHeader'>{field.Caption}</span>");

                    if (string.IsNullOrWhiteSpace(_exportMode) && option.FilteringEnabled && field.Filterable &&
                        (!string.IsNullOrWhiteSpace(field.FieldName) || !string.IsNullOrWhiteSpace(field.FilterField)))
                    {
                        var fieldType = string.IsNullOrWhiteSpace(field.FilterField)
                                       ? Digger.GetType(firstItemType, field.FieldName)
                                       : Digger.GetType(firstItemType, field.FilterField);
                        if (_filter.CanFilterable(fieldType))
                        {
                            var fReplace = field.FilterField.IsEmpty()
                                               ? field.FieldName.Replace(".", "__")
                                               : field.FilterField.Replace(".", "__");
                            builder.AppendHtml(string.Format("<a href='#filter_{0}' data-toggle='modal' class='textHeader filtre-image'><img class='tableImg' src='" +
                                    urlHelper.Content(@"~/Content/admin/plugins/mvcgrid/images/filter.png")
                                    + "' /></a>", fReplace));
                        }
                    }
                    builder.AppendHtml("</div>");
                }
                else if (field.DataColumnType == DataColumnTypes.SelectColumn)
                {
                    builder.AppendHtml("<input type='checkbox' id='chkGridSelectAllRow' />");
                }
                else
                    builder.AppendHtml($"<span class='textHeader'>{field.Caption}</span>");
                builder.AppendHtml("</th>");
            }
            builder.AppendHtml("</tr>");
            return builder.GetString();
        }

        private static string RenderFilterRow(IEnumerable<DataColumn> columns, Type firstItemType)
        {
            return "";
        }

        private static string RenderRowButtons(IHtmlHelper helper, object item, DataGridOptions option)
        {
            var builder = new HtmlContentBuilder();
            if (!option.ButtonRefreshEnabled && !option.ButtonDeleteEnabled && !option.ButtonEditEnabled
                && option.Buttons.Count <= 0)
            {
                return "";
            }
            builder.AppendHtml("<td>");
            builder.AppendHtml("<div class='btn-group'>");

            var keyFieldValue = ValueConverter.GetFieldValue(item, option.KeyField);
            var keyText = ValueConverter.GetFieldValue(item, option.TextField);
            foreach (var button in option.Buttons)
            {
                builder.AppendHtml(button.GetString()
                                    .Replace("KEYFIELD", keyFieldValue)
                                    .Replace("KEYTEXT", keyText)
                                    .Replace("btn-mini", "btn-xs"));
            }
            if (option.RowOtherButtons.Count > 0)
            {
                builder.AppendHtml(@"<div class=""btn-group""><a class=""btn btn-xs dropdown-toggle text-wrench"" data-toggle=""dropdown"" href=""#"">" +
                   "<i class=\"fa fa-wrench\"></i> "
                    + @"</a><ul class=""dropdown-menu"">");
                foreach (var button in option.RowOtherButtons)
                {
                    builder.AppendHtml("<li>" + button.GetString().Replace("KEYFIELD", keyFieldValue).Replace("KEYTEXT", keyText) + "</li>");
                }
                builder.AppendHtml("</ul></div>");
            }
            if (option.ButtonEditEnabled)
            {
                var drawEditButton = false;
                if (option.RowFormatClass != null && !option.ButtonEditShowFunction.IsEmpty())
                {
                    var obj = Activator.CreateInstance(option.RowFormatClass);
                    var a = option.RowFormatClass.GetMethod(option.ButtonEditShowFunction);
                    drawEditButton = (bool)a.Invoke(obj, new dynamic[1] { item });
                }
                else
                {
                    drawEditButton = true;
                }
                if (drawEditButton)
                {
                    var buttonClass = option.EditPopup ? option.ButtonEditPopupCss : option.ButtonEditCss;
                    var pop = option.EditPopup ? option.Popup : "";
                    var url = urlHelper.Content(string.Format(@"~/{0}/{1}/{2}",
                        LibGeneral.GetContentUrl(helper.ViewContext.RouteData),
                        option.ButtonEditAction,
                        keyFieldValue));
                    var action = url;
                    if (option.GridDiv != "divList")
                    {
                        if (action.Contains("?"))
                            action += "&";
                        else
                            action += "?";
                        action += "grid=" + option.GridDiv;
                    }
                    builder.AppendHtml($"<a href='{action}' class='{buttonClass}' {pop}><span class='fa fa-pencil'></span></a>");
                }
            }
            if (option.ButtonDeleteEnabled)
            {
                var drawDeleteButton = false;
                if (option.RowFormatClass != null && !option.ButtonDeleteShowFunction.IsEmpty())
                {
                    var obj = Activator.CreateInstance(option.RowFormatClass);
                    var a = option.RowFormatClass.GetMethod(option.ButtonDeleteShowFunction);
                    drawDeleteButton = (bool)a.Invoke(obj, new dynamic[1] { item });
                }
                else
                {
                    drawDeleteButton = true;
                }
                if (drawDeleteButton)
                {
                    var url = urlHelper.Content(string.Format(@"~/{0}/{1}/{2}",
                        LibGeneral.GetContentUrl(helper.ViewContext.RouteData),
                        option.ButtonDeleteAction,
                        keyFieldValue));
                    var action = url;
                    if (action.Contains("?"))
                        action += "&";
                    else
                        action += "?";
                    action += "grid=" + option.GridDiv;

                    var text = string.Format(Text.AskDelete, keyText).Replace("'", "\"");
                    builder.AppendHtml(string.Format(
                        "<a href='javascript:;' data-posturl='{0}' class='btnGridDelete {1}' data-confirm='{2}'><span class='fa fa-trash-o'></span></a>",
                        action,
                        option.ButtonDeleteCss,
                        text));
                }
            }
            builder.AppendHtml("</div>");
            builder.AppendHtml("</td>");
            return builder.GetString();
        }

        private static string RenderRowButtons(IEnumerable<DataGridCommand> commands)
        {
            var builder = new HtmlContentBuilder();
            if (commands == null || commands.Count() == 0)
            {
                builder.AppendHtml("<td class='btn-group'></td>");
                return builder.GetString();
            }
            builder.AppendHtml("<td class='btn-group'>");
            foreach (var command in commands)
            {
                if (command.Ajax)
                    builder.AppendHtml(string.Format("<a href='{0}' class='{1}' data-ajax='true' data-ajax-method='{2}' data-ajax-confirm='{3}'>{4}</a>",
                                      command.Url,
                                      command.Css,
                                      command.HttpMethod,
                                      command.Confirm,
                                      command.Title));
                else
                    builder.AppendHtml($"<a href='{command.Url}' class='{command.Css}'>{command.Title}</a>");
            }
            builder.AppendHtml("</td>");
            return builder.GetString();
        }

        private static string RenderRow(IHtmlHelper helper, DataColumn[] columns, object item, DataGridOptions option)
        {
            var builder = new HtmlContentBuilder();
            var idx = "";
            var idclass = "";
            if (option.RowFormatClass != null && !option.RowFormatFunction.IsEmpty())
            {
                var obj = Activator.CreateInstance(option.RowFormatClass);
                var a = option.RowFormatClass.GetMethod(option.RowFormatFunction);
                var sonuc = (string)a.Invoke(obj, new dynamic[1] { item });
                idclass = $"class='{sonuc}'";
            }
            builder.AppendHtml($"<tr {idx} {idclass}>");
            if (string.IsNullOrWhiteSpace(_exportMode))
            {
                builder.AppendHtml(RenderRowButtons(helper, item, option));
                foreach (var column in columns.Where(d => d.Visible && d.DataColumnType == DataColumnTypes.CommandColumn))
                {
                    var val = Digger.GetObjectValue(item, column.FieldName);
                    if (val is List<DataGridCommand>)
                        builder.AppendHtml(RenderRowButtons(val as List<DataGridCommand>));
                }
            }

            foreach (var field in columns.Where(d => d.Visible && d.DataColumnType != DataColumnTypes.CommandColumn))
            {
                if (field.Width > 0)
                    builder.AppendHtml($"<td width='{field.Width}'>");
                else
                    builder.AppendHtml("<td>");
                if (string.IsNullOrWhiteSpace(field.ObjectValueFunction) || field.ObjectValueConverter == null)
                    builder.AppendHtml(ValueConverter.GetValue(helper, item, field));
                else
                {
                    var type = field.ObjectValueConverter;
                    var obj = Activator.CreateInstance(type);
                    var a = type.GetMethod(field.ObjectValueFunction);
                    var sonuc = (string)a.Invoke(obj, new object[2] { helper, item });
                    builder.AppendHtml(sonuc);
                }
                builder.AppendHtml("</td>");
            }
            builder.AppendHtml("</tr>");
            return builder.GetString();
        }

        private static string RenderPager<T>(IHtmlHelper helper, IEnumerable<T> items,
            DataGridOptions option, int currentPageSize)
            where T : class
        {
            var pagedList = (IPagedList<T>)items;
            if (pagedList != null)
            {
                var urlOperator = option.PagerAndShortAction.IndexOf("?") > -1 ? "&" : "?";

                var qc = new List<KeyValuePair<string, string>>(_queryItems);
                qc.RemoveAll(d => d.Key == option.PagerKeyword);
                qc.RemoveAll(d => d.Key == option.PageSizeKeyword);

                var qb = new QueryBuilder(qc);
                var pageSizeQs = qb.ToQueryString();
                var pgeIndexli = pageSizeQs.ToUriComponent();

                var ci = option.PagerAndShortAction.Split('?');
                if (ci.Count() > 1)
                {
                    pgeIndexli = pgeIndexli.Replace(ci[1], "");
                }
                if (pgeIndexli == "?")
                    pgeIndexli = "";

                if (pgeIndexli.Length > 1)
                    pgeIndexli = "&" + pgeIndexli.Substring(1);
                pgeIndexli = pgeIndexli.Replace("&&", "&");

                var pageUrl = string.Format(@"~/{0}/{1}{6}{2}=[PAGE]&{4}={5}{3}",
                        LibGeneral.GetContentUrl(helper.ViewContext.RouteData),
                        option.PagerAndShortAction,
                        option.PagerKeyword,
                        pgeIndexli,
                        option.PageSizeKeyword,
                        currentPageSize,
                        urlOperator).Replace("[PAGE]", "{0}");

                var fs = urlHelper.Content(string.Format(@"~/{0}/{1}{4}{2}&{3}=XXX",
                            LibGeneral.GetContentUrl(helper.ViewContext.RouteData),
                            option.PagerAndShortAction,
                            pgeIndexli, //pageSizeQs,
                            option.PageSizeKeyword,
                            urlOperator));
                fs = fs.Replace("?&", "?");
                fs = fs.Replace("&&", "&");
                fs = fs.Replace("??", "?");
                var pageSizeUrl = $"{option.PagerJsFunction}('{fs}', '{option.GridDiv}');";
                return HtmlHelper2.PagedListPager2(
                     helper,
                     pagedList,
                     page => string.Format("javascript:{0}('{1}','{2}');",
                     option.PagerJsFunction,
                     urlHelper.Content(string.Format(pageUrl, page)), option.GridDiv),
                     new PagedListRenderOptions
                     {
                         DisplayItemSliceAndTotal = true
                     },
                     currentPageSize,
                     pageSizeUrl,
                     option.ItemListCountList);
            }
            return string.Empty;
        }

        private static string RenderFilterRowFooter(IEnumerable<DataColumn> columns, DataGridOptions option)
        {
            return _filter.ActiveFilter(_colCount, columns.ToList());
        }

        private static string RenderScript(IHtmlHelper helper, DataGridOptions option)
        {
            return "";
        }

        private static string RenderNextScrollLink(IHtmlHelper helper, DataGridOptions option, int pageIndex)
        {
            var builder = new HtmlContentBuilder();

            var linkTag = @"<tr class='kScrollNav hide'>
        <td><a href='{0}'>NEXT</a></td>
    </tr>";
            var nextUrl = string.Empty;
            if (pageIndex + 1 <= TotalPageCount)
            {
            }
            builder.AppendHtml(string.Format(linkTag, nextUrl));
            return builder.GetString();
        }
    }
}