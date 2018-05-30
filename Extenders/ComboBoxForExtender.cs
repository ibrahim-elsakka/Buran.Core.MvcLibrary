using Buran.Core.Library.Utils;
using Buran.Core.MvcLibrary.Data.Attributes;
using Buran.Core.MvcLibrary.Reflection;
using Buran.Core.MvcLibrary.Utils;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Buran.Core.MvcLibrary.Extenders
{
    public class ComboBoxDataInfo
    {
        public SelectList ListItems { get; set; }
        public bool CanSelect { get; set; }
    }

    public static class ComboBoxForExtender
    {
        private static ComboBoxDataInfo GetComboBoxDataSource(ModelExplorer metadata)
        {
            ComboBoxDataInfo result = new ComboBoxDataInfo();
            var comboDataModel = Digger2.GetMetaAttr<ComboBoxDataAttribute>(metadata.Metadata);
            if (comboDataModel != null)
            {
                result.CanSelect = comboDataModel.ShowSelect;

                if (comboDataModel.Repository != null)
                {
                    var repo = comboDataModel.Repository;
                    if (repo != null)
                    {
                        var obj = Activator.CreateInstance(repo);
                        var a = repo.GetMethod(comboDataModel.QueryName);
                        if (a == null)
                            return null;
                        if (a.GetParameters().Count() == 1)
                        {
                            var dataList = a.Invoke(obj, new object[1] { metadata.Model });
                            result.ListItems = dataList as SelectList;
                        }
                        else if (a.GetParameters().Count() == 2)
                        {
                            var dataList = a.Invoke(obj, new object[2] { metadata.Model, false });
                            result.ListItems = dataList as SelectList;
                        }
                    }
                }
            }
            return result;
        }

        public static HtmlString ComboBoxFor<TModel, TValue>(this IHtmlHelper<TModel> html,
            Expression<Func<TModel, TValue>> expression, IEnumerable<SelectListItem> items = null,
            string optionLabel = null, object htmlAttributes = null, string dataUrl = null)
        {
            var select = new TagBuilder("select");
            if (!dataUrl.IsEmpty())
                select.Attributes.Add("data-url", dataUrl);
            if (htmlAttributes != null)
                select.MergeAttributes(new RouteValueDictionary(htmlAttributes));

            var metadata = ExpressionMetadataProvider.FromLambdaExpression(expression, html.ViewData, html.MetadataProvider);
            var htmlFieldName = ExpressionHelper.GetExpressionText(expression);
            var labelText = metadata.Metadata.DisplayName ?? metadata.Metadata.PropertyName ?? htmlFieldName.Split('.').Last();
            if (items == null)
            {
                var comboDataInfo = GetComboBoxDataSource(metadata);
                items = comboDataInfo.ListItems;
                if (comboDataInfo.CanSelect)
                    optionLabel = "SELECT";
                else
                    optionLabel = null;
            }
            select.Attributes.Add("name", htmlFieldName);
            select.Attributes.Add("class", "form-control input-sm");
            select.GenerateId(htmlFieldName, "_");

            var sb = new StringBuilder();
            if (!optionLabel.IsEmpty())
                sb.AppendLine(string.Concat("<option value=\"\">", optionLabel, "</option>"));
            if (items != null)
                foreach (var item in items)
                {
                    var option = new TagBuilder("option");
                    option.Attributes.Add("value", html.Encode(item.Value));
                    if (item.Selected)
                        option.Attributes.Add("selected", "selected");
                    option.InnerHtml.SetHtmlContent(item.Text);
                    sb.AppendLine(option.GetString());
                }
            select.InnerHtml.SetHtmlContent(sb.ToString());

            return new HtmlString(string.Format(@"<div class=""form-group"">
        <label class=""col-sm-3 control-label"">{0}</label>
        <div class=""col-sm-8"">
            {1}
        </div>
    </div>", labelText, select.GetString()));
        }


        public static HtmlString Select2DropDownListFor<TModel, TValue>(this IHtmlHelper<TModel> html,
            Expression<Func<TModel, TValue>> expression, IEnumerable<SelectListItem> items = null,
            string placeHolder = null, object htmlAttributes = null, bool multiSelect = false, bool canClearSelect = false)
        {
            var select = new TagBuilder("select");
            if (htmlAttributes != null)
                select.MergeAttributes(new RouteValueDictionary(htmlAttributes));

            var metadata = ExpressionMetadataProvider.FromLambdaExpression(expression, html.ViewData, html.MetadataProvider);
            var htmlFieldName = metadata.Metadata.PropertyName;
            var labelText = metadata.Metadata.DisplayName ?? metadata.Metadata.PropertyName ?? htmlFieldName.Split('.').Last();

            select.Attributes.Add("name", htmlFieldName);
            select.Attributes.Add("id", htmlFieldName);
            select.Attributes.Add("class", "form-control input-sm");
            if (multiSelect)
                select.Attributes.Add("multiple", "multiple");

            var sb = new StringBuilder();
            if (items != null)
                foreach (var item in items)
                {
                    var option = new TagBuilder("option");
                    option.Attributes.Add("value", html.Encode(item.Value));
                    if (item.Selected)
                        option.Attributes.Add("selected", "selected");
                    option.InnerHtml.SetHtmlContent(item.Text);
                    sb.AppendLine(option.GetString());
                }
            select.InnerHtml.SetHtmlContent(sb.ToString());

            var div = string.Format(@"<div class=""form-group"">
        <label class=""col-sm-3 control-label"">{0}</label>
        <div class=""col-sm-8"">
            {1}
        </div>
    </div>", labelText, select.GetString());

            string js = string.Format(@"<script type=""text/javascript"">
$(function () {{
    $(""#{0}"").select2({{
        placeholder: ""{1}"",
        {2}
    }});
}});
</script>", htmlFieldName,
                placeHolder.IsEmpty() ? "" : placeHolder,
                canClearSelect ? "allowClear: true" : "");
            return new HtmlString(div + js);
        }
    }
}
