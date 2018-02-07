using Buran.Core.Library.Utils;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Buran.Core.MvcLibrary.Extenders
{
    public static class CheckListBoxExtender
    {
        public class CheckBoxListInfo
        {
            public CheckBoxListInfo(string value, string displayText, bool isChecked, string data)
            {
                Value = value;
                Data = data;
                DisplayText = displayText;
                IsChecked = isChecked;
            }

            public CheckBoxListInfo(string value, string displayText, bool isChecked)
            {
                Value = value;
                DisplayText = displayText;
                IsChecked = isChecked;
            }

            public CheckBoxListInfo()
            {
            }

            public string Value { get; set; }
            public string Data { get; set; }
            public string DisplayText { get; set; }
            public bool IsChecked { get; set; }
        }

        public static HtmlString CheckBoxList(this IHtmlHelper helper, string name, List<CheckBoxListInfo> listInfo)
        {
            return helper.CheckBoxList(name, listInfo, null);
        }

        public static HtmlString CheckBoxList(this IHtmlHelper helper, string name, List<CheckBoxListInfo> listInfo,
           object htmlAttributes)
        {
            return helper.CheckBoxList(name, listInfo, new RouteValueDictionary(htmlAttributes), string.Empty);
        }

        public static HtmlString CheckBoxList(this IHtmlHelper helper, string name, List<CheckBoxListInfo> listInfo,
          IDictionary<string, object> htmlAttributes, string img)
        {
            if (String.IsNullOrEmpty(name))
                throw new ArgumentException("The argument must have a value", "name");
            if (listInfo == null)
                throw new ArgumentNullException("listInfo");

            var sb = new StringBuilder();
            foreach (var info in listInfo)
            {
                var label = new TagBuilder("label");
                label.AddCssClass("checkbox");

                var builder = new TagBuilder("input");
                if (info.IsChecked) builder.MergeAttribute("checked", "checked");
                builder.MergeAttributes(htmlAttributes);
                builder.MergeAttribute("type", "checkbox");
                builder.MergeAttribute("value", info.Value);
                builder.MergeAttribute("name", name);
                builder.InnerHtml.AppendHtml(info.DisplayText);

                label.InnerHtml.AppendHtml(builder);
                label.InnerHtml.AppendHtml(img.Replace("##VAL##", info.Value));

                sb.Append(label);
            }
            return new HtmlString(sb.ToString());
        }

        public static HtmlString CheckBoxListFor(this IHtmlHelper html, string name, string labelText,
                List<CheckBoxListInfo> listInfo, string cssClass = null, IDictionary<string, object> htmlAttributes = null, string img = null)
        {
            if (listInfo == null)
                throw new ArgumentNullException("Empty List");

            var sb = new StringBuilder();
            var i = 1;
            foreach (var info in listInfo)
            {
                var label = new TagBuilder("label");
                label.AddCssClass("checkbox");

                var builder = new TagBuilder("input");
                if (info.IsChecked)
                    builder.MergeAttribute("checked", "checked");
                if (htmlAttributes != null)
                    builder.MergeAttributes(htmlAttributes);
                builder.MergeAttribute("type", "checkbox");
                builder.MergeAttribute("value", info.Value);
                builder.MergeAttribute("name", name);
                builder.MergeAttribute("class", cssClass);

                if (!info.Data.IsEmpty())
                    builder.MergeAttribute("data-dt", info.Data);

                builder.MergeAttribute("id", name + "_" + i);
                builder.InnerHtml.AppendHtml(info.DisplayText);

                if (!img.IsEmpty())
                {
                    label.InnerHtml.AppendHtml(builder);
                    label.InnerHtml.AppendHtml(img.Replace("##VAL##", info.Value));
                }
                else
                    label.InnerHtml.AppendHtml(builder);

                sb.Append(label);
                i++;
            }

            var div = string.Format(@"<div class='form-group' id='div{0}'>
    <label for='{0}' class='col-sm-3 control-label'>{1}</label>
    <div class='col-sm-8 checkListBox'>
        {2}
    </div>
</div>",
               name,
               labelText,
               sb
               );

            return new HtmlString(div);
        }
    }
}
