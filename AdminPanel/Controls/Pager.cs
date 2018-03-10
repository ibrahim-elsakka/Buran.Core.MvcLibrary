using System.Collections.Generic;
using System.Text;
using Buran.Core.Library.Utils;
using Buran.Core.MvcLibrary.AdminPanel.Controls;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Buran.Core.MvcLibrary.AdminPanel
{
    public static class Pager
    {
        internal static void AddMenu(StringBuilder sb, List<EditorPageMenuSubItem> menu)
        {
            foreach (var item in menu)
            {
                sb.AppendLine(string.Format(@"<li><a href='{0}'>{1}</a></li>", item.Url, item.Title));
            }
        }

        public static HtmlString PageMenu(this IHtmlHelper helper, EditorPageMenu menu)
        {
            var sb = new StringBuilder();

            sb.AppendLine("<div class='hidden-xs'>");
            foreach (var item in menu.Items)
            {
                if (item.Items.Count == 0)
                {
                    if (item.Id.IsEmpty())
                        sb.AppendLine(string.Format(@"<a href='{0}' class='btn btn-default {3}'>
    <i class='{1}'></i> <span>{2}</span>
</a>", item.Url, item.IconClass, item.Title, item.ButtonClass));
                    else
                        sb.AppendLine(string.Format(@"<button id='{0}' class='btn green-haze  {3}'>
<i class='{1}'></i> <span>{2}</span>
</button>",
                            item.Id, item.IconClass, item.Title, item.ButtonClass));
                }
                else
                {
                    sb.AppendLine(string.Format(@"<div class='btn-group'>
<a class='btn btn-default {2}' href='#' data-toggle='dropdown'>
    <i class='{1}'></i> {0} <i class='fa fa-angle-down'></i>
</a>
<ul class='dropdown-menu pull-right'>", item.Title, item.IconClass, item.ButtonClass));
                    AddMenu(sb, item.Items);
                    sb.AppendLine("</ul></div>");
                }
            }
            sb.AppendLine("</div>");

            sb.AppendLine("<div class='visible-xs'>");
            sb.AppendLine(string.Format(@"<div class='btn-group'>
<a class='btn btn-default {2}' href='#' data-toggle='dropdown'>
    <i class='{1}'></i> {0} <i class='fa fa-angle-down'></i>
</a>
<ul class='dropdown-menu pull-right'>", "İşlemler", "", ""));
            foreach (var item in menu.Items)
            {
                if (item.Items.Count == 0)
                {
                    if (item.Id.IsEmpty())
                        sb.AppendLine(string.Format(@"<li><a href='{0}' class='{3}'>
    <i class='{1}'></i> <span>{2}</span>
</a></li>", item.Url, item.IconClass, item.Title, item.ButtonClass));
                    else
                        sb.AppendLine(string.Format(@"<li><button id='{0}' class='btn btn-link {3}'>
<i class='{1}'></i> <span>{2}</span>
</button></li>",
                            item.Id, item.IconClass, item.Title, item.ButtonClass));
                }
            }
            sb.AppendLine("</ul>");
            sb.AppendLine("</div>");
            sb.AppendLine("</div>");
            return new HtmlString(sb.ToString());
        }
    }
}
