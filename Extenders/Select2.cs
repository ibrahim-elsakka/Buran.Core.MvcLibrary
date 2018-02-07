using Buran.Core.Library.Utils;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Buran.Core.MvcLibrary.Extenders
{
    public class TokenInputValue
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public static class Select2
    {
        public static HtmlString Select2For<TModel, TValue>(this IHtmlHelper<TModel> html,
            Expression<Func<TModel, TValue>> expression,
            string url, int resultSize = 0, bool multiselect = true,
            List<TokenInputValue> initalValues = null, bool showLabel = true,
            string waterMark = "Select", string cssclass = "input-large", string loaderUrl = null)
        {
            var metadata = ExpressionMetadataProvider.FromLambdaExpression(expression, html.ViewData, html.MetadataProvider);
            var htmlFieldName = ExpressionHelper.GetExpressionText(expression);
            var labelText = metadata.Metadata.DisplayName ?? metadata.Metadata.PropertyName ?? htmlFieldName.Split('.').Last();
            return Select2For(metadata.Metadata.PropertyName, labelText, url, resultSize, multiselect, initalValues, showLabel, waterMark, cssclass, loaderUrl);
        }

        public static HtmlString Select2For(this IHtmlHelper html,
            string name, string caption, string url,
            int resultSize = 0,
            bool multiselect = true,
            List<TokenInputValue> initalValues = null, bool showLabel = true,
            string waterMark = "Select", string cssclass = "input-large", string loaderUrl = null)
        {
            return Select2For(name, caption, url, resultSize, multiselect, initalValues, showLabel, waterMark, cssclass, loaderUrl);
        }

        public static HtmlString Select2For(string name, string caption, string url,
           int resultSize = 0, bool multiselect = true, List<TokenInputValue> initalValues = null,
           bool showLabel = true, string waterMark = "Select", string cssclass = "input-large",
           string loaderUrl = null)
        {
            var div = showLabel
                          ? string.Format(@"<div class='form-group'>
            <label for='{0}' class='col-sm-3 control-label'>{1}</label>
            <div class='col-sm-8'>
                <input id='{0}' name='{0}' data-url='{3}' type='text' class='form-control {2}'>
            </div>
        </div>", name, caption, cssclass, loaderUrl)
                          : string.Format("<input id='{0}' name='{0}' type='text' data-url=\"{1}\" class='{2}'>",
                                name, loaderUrl, cssclass);

            var initalJson = string.Empty;
            var json = "";
            if (initalValues != null && initalValues.Count > 0)
            {
                json = "[";
                var fistItem = true;
                foreach (var value in initalValues)
                {
                    if (!fistItem)
                        json += ",";
                    json += "{id: " + value.Id + ", text: \"" + value.Name + "\"}";
                    fistItem = false;
                }
                json += "]";
                initalJson = string.Format("$('#{0}').select2('data', {1});", name, json);
            }

            string js = string.Format(@"<script type=""text/javascript"">
        $(function () {{
            $(""#{0}"").select2({{
                placeholder: ""{5}"",
                allowClear: true,
                multiple:true,
                {4}
                minimumInputLength: 1,
                ajax: {{
                    url: '{1}',
                    dataType: 'json',
                    data: function (term, page) {{
                        return {{
                            q: term,
                            size: {2},
                        }};
                    }},
                    results: function (data, page) {{
                        return {{ results: data }};
                    }}
                }}
            }});
            {3}
        }});</script>", name, url, resultSize, initalJson, multiselect ? "" : "maximumSelectionSize:1,", waterMark);
            return new HtmlString(div + js);
        }

        public static HtmlString Select2ComboFor<TModel, TValue>(this IHtmlHelper<TModel> html,
            Expression<Func<TModel, TValue>> expression,
            string url, string loaderUrl, string waterMark = "Select",
            string cssclass = "input-sm", string parentCombo = "",
            bool editorTemplate = true)
        {
            var metadata = ExpressionMetadataProvider.FromLambdaExpression(expression, html.ViewData, html.MetadataProvider);
            var htmlFieldName = ExpressionHelper.GetExpressionText(expression);
            var labelText = metadata.Metadata.DisplayName ?? metadata.Metadata.PropertyName ?? htmlFieldName.Split('.').Last();
            return Select2ComboFor(html, metadata.Metadata.PropertyName, labelText, metadata.Model, url, loaderUrl,
                waterMark, cssclass, parentCombo, editorTemplate);
        }

        public static HtmlString Select2ComboFor(this IHtmlHelper html,
            string name, string caption, object value, string url, string loaderUrl,
            string waterMark = "Select", string cssclass = "input-sm",
            string parentCombo = "", bool editorTemplate = true)
        {
            var div = "";
            if (editorTemplate)
            {
                div = string.Format(@"<div class='form-group' id='div{0}'>
    <label for='{0}' class='col-sm-3 control-label'>{1}</label>
    <div class='col-sm-8'>
        <input id='{0}' name='{0}' type='text' class='form-control {2}' {3}>
    </div>
</div>", name, caption, cssclass, value != null ? "value='" + value + "'" : ""
               );
            }
            else
            {
                div = string.Format(@"<input id='{0}' name='{0}' type='text' class='form-control {1}' {2}>",
                  name, cssclass, value != null ? "value='" + value + "'" : ""
              );
            }

            string js = string.Format(@"<script type=""text/javascript"">
$(function () {{
    $(""#{0}"").select2({{
        placeholder: ""{3}"",
        minimumInputLength: 1,
        ajax: {{
            url: '{1}',
            dataType: 'json',
            data: function (term, page) {{
                return {{
                    q: term,
                    {2}
                }};
            }},
            results: function (data, page) {{
                return {{ results: data }};
            }},
            cache: true
        }},
        initSelection: function(element, callback) {{
            var id = $(element).val();
            if (id != '') {{
                $.ajax('{4}/' + id, {{
                    dataType: 'json'
                }}).done(function(data) {{ callback(data); }});
            }}
        }},
    }});
}});
</script>", name, url, !parentCombo.IsEmpty() ? string.Format("id: $('#{0}').val(),", parentCombo) : "", waterMark, loaderUrl);
            return new HtmlString(div + js);
        }

        public static HtmlString Select2ComboFor1<TModel, TValue>(this IHtmlHelper<TModel> html,
            Expression<Func<TModel, TValue>> expression,
            string selectedText, string cssclass = "input-sm")
        {
            var metadata = ExpressionMetadataProvider.FromLambdaExpression(expression, html.ViewData, html.MetadataProvider);
            var htmlFieldName = ExpressionHelper.GetExpressionText(expression);
            var labelText = metadata.Metadata.DisplayName ?? metadata.Metadata.PropertyName ?? htmlFieldName.Split('.').Last();
            return Select2ComboFor1(html, metadata.Metadata.PropertyName, labelText, metadata.Model, selectedText, cssclass);
        }

        public static HtmlString Select2ComboFor1(this IHtmlHelper html, string name, string caption, object value,
            string selectedText, string cssclass = "input-sm")
        {
            var div = string.Format(@"<div class='form-group' id='div{0}'>
    <label for='{0}' class='col-sm-3 control-label'>{1}</label>
    <div class='col-sm-8'>
        <select  id='{0}' name='{0}' class='form-control {2}'>
 {3}
        </select>
    </div>
</div>",
                name,
                caption,
                cssclass,
                value != null ? "<option value='" + value + "'>" + selectedText + "</option>" : ""
                );
            return new HtmlString(div);
        }

        public static HtmlString Select2ComboFor2<TModel, TValue>(this IHtmlHelper<TModel> html, 
            Expression<Func<TModel, TValue>> expression,
            string url, string waterMark = "Select", string cssclass = "input-sm", string parentCombo = "")
        {
            var metadata = ExpressionMetadataProvider.FromLambdaExpression(expression, html.ViewData, html.MetadataProvider);
            return Select2ComboFor2(html, metadata.Metadata.PropertyName, url, waterMark, parentCombo);
        }

        public static HtmlString Select2ComboFor2(this IHtmlHelper html, string name, string url,
          string waterMark = "Select", string parentCombo = "")
        {
            string js = string.Format(@"<script type=""text/javascript"">
$(function () {{
    $(""#{0}"").select2({{
        placeholder: ""{3}"",
        minimumInputLength: 1,
        ajax: {{
            url: '{1}',
            dataType: 'json',
            data: function (params) {{
                  return {{
                    q: params.term,
                    {2}
                  }};
            }},
            processResults: function (data, params) {{
                return {{
                    results: data 
                }};
            }},
            cache: true
        }},
    }});
}});
</script>", name, url, !parentCombo.IsEmpty() ? string.Format("id: $('#{0}').val(),", parentCombo) : "", waterMark);
            return new HtmlString(js);
        }
    }
}
