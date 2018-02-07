
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Buran.Core.MvcLibrary.Extenders
{
    public static class LabelRowExtender
    {
        public static HtmlString LabelRow(this IHtmlHelper html, string title, string content)
        {
            return new HtmlString(string.Format(@"<div class=""form-group"">
        <label class=""col-sm-3 control-label"">{0}</label>
        <div class=""col-sm-8"">
            <p class=""form-control-static"">{1}</p>
        </div>
    </div>", title, content));
        }
    }
}