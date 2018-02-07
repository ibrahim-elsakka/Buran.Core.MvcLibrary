using Buran.Core.Library.Reflection;
using Buran.Core.MvcLibrary.Grid.Columns;
using Buran.Core.MvcLibrary.Resource;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Primitives;

namespace Buran.Core.MvcLibrary.Grid.Helper
{
    class FilterInfo
    {
        public string Keyword { get; set; }
        public string Condition { get; set; }
        public string Value { get; set; }
        public string FilterDesc { get; set; }
        public string Key { get; set; }
        public string FilterType { get; set; }
    }

    class FilterWhere
    {
        public string Where { get; set; }
        public List<object> Params { get; set; }

        public FilterWhere()
        {
            Params = new List<object>();
        }
    }

    class Filter
    {
        private readonly string _pagerKeyword;
        private readonly RouteData _routeData;
        private readonly string _dataAction;
        private UrlHelper urlHelper { get; set; }
        private readonly string _jsFunction;
        private readonly string _divGrid;
        private Dictionary<string, StringValues> Query { get; }
        private List<FilterInfo> List { get; }

        public Filter(Dictionary<string, StringValues> query, string pagerKeyword, RouteData routeData,
            string dataAction, IHtmlHelper helper, string jsFunction, string divGrid)
        {
            _pagerKeyword = pagerKeyword;
            _routeData = routeData;
            _dataAction = dataAction;
            urlHelper = new UrlHelper(helper.ViewContext);
            _jsFunction = jsFunction;
            _divGrid = divGrid;
            Query = query;
            List = new List<FilterInfo>();

            foreach (var key in query.Keys)
            {
                if (key == null || !key.StartsWith("s_")) continue;

                if (string.IsNullOrWhiteSpace(query[key]))
                    continue;

                var field = key.Replace("__", ".").Replace("s_", "");
                var value = query[key].ToString().ToLower();
                var kriter = query[key.Replace("s_", "c_")].ToString().ToLower();
                var ft = query[key.Replace("s_", "ft_")].ToString().ToLower();

                List.Add(new FilterInfo { Keyword = field, Condition = kriter, Value = value, Key = key, FilterType = ft });
            }
        }
        internal string GetKriter(Type fieldType, int kriterId, string caption, string name)
        {
            var builder = new StringBuilder();
            if (fieldType == typeof(string))
            {
                builder.AppendLine(string.Format("<li><a href='#' class='btnCriSelect' data-fname='{2}' data-fid='7' data-ftext='{1}'>{0}</a></li>",
                    Text.Iceren, caption + " " + Text.Iceren, name));
                builder.AppendLine(string.Format("<li><a href='#' class='btnCriSelect' data-fname='{2}' data-fid='1' data-ftext='{1}'>{0}</a></li>",
                    Text.Esit, caption + " " + Text.Esit, name));
                builder.AppendLine(string.Format("<li><a href='#' class='btnCriSelect' data-fname='{2}' data-fid='2' data-ftext='{1}'>{0}</a></li>",
                    Text.Kucuktur, caption + " " + Text.Kucuktur, name));
                builder.AppendLine(string.Format("<li><a href='#' class='btnCriSelect' data-fname='{2}' data-fid='3' data-ftext='{1}'>{0}</a></li>",
                    Text.Buyuktur, caption + " " + Text.Buyuktur, name));
                builder.AppendLine(string.Format("<li><a href='#' class='btnCriSelect' data-fname='{2}' data-fid='4' data-ftext='{1}'>{0}</a></li>",
                    Text.KucukEsit, caption + " " + Text.KucukEsit, name));
                builder.AppendLine(string.Format("<li><a href='#' class='btnCriSelect' data-fname='{2}' data-fid='5' data-ftext='{1}'>{0}</a></li>",
                    Text.BuyukEsit, caption + " " + Text.BuyukEsit, name));
                builder.AppendLine(string.Format("<li><a href='#' class='btnCriSelect' data-fname='{2}' data-fid='6' data-ftext='{1}'>{0}</a></li>",
                    Text.Baslayan, caption + " " + Text.Baslayan, name));

            }
            return builder.ToString();
        }

        public string GetFilterDiv(DataColumn field, Type firstItemType)
        {
            var builder = new StringBuilder();
            var fieldName = string.IsNullOrWhiteSpace(field.FilterField)
                                ? field.FieldName
                                : field.FilterField;

            var fieldType = Digger.GetType(firstItemType, fieldName);
            if (firstItemType == null)
            {
                var fReplace = fieldName.Replace(".", "__");
                FilterInfo currentFilter = null;
                var currentFilterData = List.Where(d => d.Keyword == fReplace);
                if (currentFilterData.Any())
                    currentFilter = currentFilterData.First();

                field.FilterValue = Query["s_" + fReplace];
                var kriterId = 7;
                var kriterYazi = field.Caption + " ";
                if (currentFilter != null)
                {
                    int.TryParse(currentFilter.Condition, out kriterId);
                    switch (currentFilter.Condition)
                    {
                        case "2":
                            kriterYazi += "<";
                            break;
                        case "3":
                            kriterYazi += ">";
                            break;
                        case "4":
                            kriterYazi += "<=";
                            break;
                        case "5":
                            kriterYazi += ">=";
                            break;
                        case "6":
                            kriterYazi += "İle Başlayan";
                            break;
                        case "7":
                            kriterYazi += "İçeren";
                            break;
                        case "8":
                            kriterYazi += "İle Biten";
                            break;
                        default:
                            kriterYazi += "İçeren";
                            break;
                    }
                }
                else kriterYazi += " İçeren";
                if (currentFilter != null)
                    currentFilter.FilterDesc = kriterYazi + " " + field.FilterValue;
            }
            else
                if (CanFilterable(fieldType))
            {
                var fReplace = fieldName.Replace(".", "__");
                FilterInfo currentFilter = null;
                var currentFilterData = List.Where(d => d.Keyword == fReplace);
                if (currentFilterData.Any())
                    currentFilter = currentFilterData.First();

                field.FilterValue = Query["s_" + fReplace];
                builder.AppendLine($@"<div class='modal filterPopup' id='filter_{fReplace}'>
    <div class='modal-dialog'>
        <div class='modal-content'>
        <div class='modal-header'>
            <button type='button' class='close' data-dismiss='modal'>×</button>
            <h4 class='modal-title'>{field.Caption}</h4>
        </div>
        <div class='modal-body'>
            <div class='row'>
                <div class='col-sm-12'>
                    <div class='input-group'>
    ");
                if (fieldType == typeof(DateTime) || fieldType == typeof(DateTime?))
                {
                    var d1 = string.Empty;
                    var d2 = string.Empty;
                    if (!string.IsNullOrWhiteSpace(field.FilterValue))
                    {
                        var dates = field.FilterValue.Split('*');
                        if (dates.GetUpperBound(0) == 1)
                        {
                            d1 = dates[0];
                            d2 = dates[1];
                        }
                    }
                    builder.AppendLine(string.Format("<input type='textbox' id='s_d1_{0}' class='txtSearch {2}' value='{1}' />",
                        fReplace, d1, "datepicker"));
                    builder.AppendLine(string.Format("<input type='textbox' id='s_d2_{0}' class='txtSearch {2}' value='{1}' />",
                        fReplace, d2, "datepicker"));

                    if (currentFilter != null)
                        currentFilter.FilterDesc = field.Caption + " " + d1 + " ile " + d2 + " arasında";
                }
                else
                {
                    var conditionId = 7;
                    var conditionTitle = field.Caption + " ";
                    if (currentFilter != null)
                    {
                        int.TryParse(currentFilter.Condition, out conditionId);
                        switch (currentFilter.Condition)
                        {
                            case "2":
                                conditionTitle += "<";
                                break;
                            case "3":
                                conditionTitle += ">";
                                break;
                            case "4":
                                conditionTitle += "<=";
                                break;
                            case "5":
                                conditionTitle += ">=";
                                break;
                            case "6":
                                conditionTitle += "İle Başlayan";
                                break;
                            case "7":
                                conditionTitle += "İçeren";
                                break;
                            case "8":
                                conditionTitle += "İle Biten";
                                break;
                            default:
                                conditionTitle += "İçeren";
                                break;
                        }
                    }
                    else conditionTitle += " İçeren";

                    builder.Append(string.Format(@"<div class='input-group-btn'>
            <input type='hidden' id='c_{1}' value={2} />
            <button type='button' id='cName_{1}' class='btn btn-default btn-sm dropdown-toggle' data-toggle='dropdown'>{0} <span class='caret'></span></button>
            <ul class='dropdown-menu'>
            ", conditionTitle, fReplace, conditionId));

                    builder.AppendLine(GetKriter(fieldType, conditionId, field.Caption, fReplace));
                    builder.AppendLine("</ul></div>");
                    builder.AppendLine(string.Format("<input type='textbox' id='s_{0}' class='form-control input-sm col-sm-6 txtSearch {2}' value='{1}' data-ft='{3}' />",
                                      fReplace, field.FilterValue,
                                      fieldType == typeof(int) ? "mask-int" : "",
                                      fieldType == typeof(int) || fieldType == typeof(int?)
                                        ? "1"
                                        : fieldType == typeof(decimal) || fieldType == typeof(decimal?)
                                            ? "2"
                                            : fieldType == typeof(DateTime) || fieldType == typeof(DateTime?)
                                                ? "3"
                                                : "0"));

                    if (currentFilter != null)
                        currentFilter.FilterDesc = conditionTitle + " " + field.FilterValue;
                }
                builder.AppendLine(string.Format(@"</div>
      </div>
    </div>
    </div>
      <div class='modal-footer'>
        <button type='button' class='btn btn-default btn-xs' data-dismiss='modal'>{3}</button>
        <a id='b_{0}' class='btn btn-xs btn-primary btnSearch' data-rel1='{1}' data-rel2='{2}' data-rel3='{5}'>{4}</a>
      </div>
    </div>
    </div>
    </div>", fReplace, GetWhereUrl(fReplace), _jsFunction, Text.Close, Text.Search, _divGrid));
            }
            return builder.ToString();
        }

        internal string GetWhereUrl(string fieldName)
        {
            var query = new Dictionary<string, StringValues>(Query);
            query.Remove(_pagerKeyword);
            query.Remove("s_" + fieldName);
            query.Remove("c_" + fieldName);
            query.Remove("ft_" + fieldName);
            var qs = query.ToString(); 
            var op = _dataAction.Contains("?") ? "&" : "?";
            var url = urlHelper.Content($@"~/{LibGeneral.GetContentUrl(_routeData)}/{_dataAction}{op}{qs}");
            return url;
        }

        public FilterWhere GetWhereString()
        {
            var w = new FilterWhere();
            var paramIndex = 0;
            foreach (var item in List)
            {
                var itemWhere = string.Empty;
                if (item.FilterType == "0")
                {
                    switch (item.Condition)
                    {
                        case "1":
                            itemWhere = item.Keyword + ".ToLower()=@" + paramIndex;
                            w.Params.Add(item.Value);
                            break;
                        case "2":
                            itemWhere = item.Keyword + ".ToLower()<@" + paramIndex;
                            w.Params.Add(item.Value);
                            break;
                        case "3":
                            itemWhere = item.Keyword + ".ToLower()>@" + paramIndex;
                            w.Params.Add(item.Value);
                            break;
                        case "4":
                            itemWhere = item.Keyword + ".ToLower()<=@" + paramIndex;
                            w.Params.Add(item.Value);
                            break;
                        case "5":
                            itemWhere = item.Keyword + ".ToLower()>=@" + paramIndex;
                            w.Params.Add(item.Value);
                            break;
                        case "6":
                            itemWhere = item.Keyword + ".ToLower().StartsWith(@" + paramIndex + ")";
                            w.Params.Add(item.Value);
                            break;
                        case "7":
                            itemWhere = item.Keyword + ".ToLower().Contains(@" + paramIndex + ")";
                            w.Params.Add(item.Value);
                            break;
                    }
                    paramIndex++;
                }
                else if (item.FilterType == "1")
                {
                    itemWhere = item.Keyword + "=@" + paramIndex;
                    w.Params.Add(int.Parse(item.Value));
                    paramIndex++;
                }
                else if (item.FilterType == "2")
                {
                    itemWhere = item.Keyword + "=@" + paramIndex;
                    w.Params.Add(decimal.Parse(item.Value));
                    paramIndex++;
                }
                else if (item.FilterType == "3")
                {
                    var d1 = string.Empty;
                    var d2 = string.Empty;
                    if (!string.IsNullOrWhiteSpace(item.Value))
                    {
                        var dates = item.Value.Split('*');
                        if (dates.GetUpperBound(0) == 1)
                        {
                            d1 = dates[0];
                            d2 = dates[1];
                        }
                    }
                    itemWhere = item.Keyword + ">=@" + paramIndex;
                    paramIndex++;
                    itemWhere += " and " + item.Keyword + "<=@" + paramIndex;
                    w.Params.Add(DateTime.Parse(d1));
                    w.Params.Add(DateTime.Parse(d2));
                    paramIndex++;
                }
                if (!string.IsNullOrWhiteSpace(w.Where))
                    w.Where += " and ";
                w.Where += itemWhere;
            }
            return w;
        }

        public string ActiveFilter(int colCount, List<DataColumn> fields)
        {
            var query = new Dictionary<string, StringValues>(Query);
            query.Remove(_pagerKeyword);
            foreach (var item in List)
            {
                query.Remove(item.Key);
                query.Remove(item.Key.Replace("s_", "c_"));
                query.Remove(item.Key.Replace("s_", "ft_"));
            }
            var param = query.ToString();
            var op = "";
            op += _dataAction.Contains("?") ? "&" : "?";
            var loader = $"javascript:{_jsFunction}('/{LibGeneral.GetContentUrl(_routeData)}/{_dataAction}{op}{param}','{_divGrid}');";
            var builder = new StringBuilder();
            builder.AppendLine($"<tr colspan='{colCount.ToString(CultureInfo.InvariantCulture)}' class='filterRow'>");
            builder.AppendLine("<td>");

            builder.AppendLine(string.Format(@"<table width='100%'><tr>
            	<td width='20px'><a href=""{0}""><img src='"
                + urlHelper.Content(@"~/Content/admin/plugins/MvcGrid/images/clear.png")
                + "' alt='" + Text.ClearFilter + "' /></a></td><td>" + Text.CurrentFilter, loader));
            var firstItem = true;
            foreach (var field in List)
            {
                if (!firstItem)
                    builder.Append(", ");
                builder.AppendLine(field.FilterDesc);
                firstItem = false;
            }
            builder.AppendLine("</td></tr></table>");
            builder.AppendLine("</td>");
            builder.AppendLine("</tr>");
            return builder.ToString();
        }

        public bool CanFilterable(Type fieldType)
        {
            return fieldType == typeof(string)
                   || fieldType == typeof(int)
                   || fieldType == typeof(int?)
                   || fieldType == typeof(decimal)
                   || fieldType == typeof(decimal?)
                   || fieldType == typeof(DateTime)
                   || fieldType == typeof(DateTime?);
        }
    }
}
