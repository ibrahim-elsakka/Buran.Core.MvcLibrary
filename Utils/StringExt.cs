using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;
using System.Text.Encodings.Web;
using System.Web;

namespace Buran.Core.MvcLibrary.Utils
{
    public static class StringExt
    {
        public static string JsAlert(string text)
        {
            return "alert(\"" + HttpUtility.JavaScriptStringEncode(text.Replace("\r\n", "")) + "\");";
        }

        public static string GetString(this TagBuilder tag)
        {
            var writer = new StringWriter();
            tag.WriteTo(writer, HtmlEncoder.Default);
            return writer.ToString();
        }

        public static string GetString(this IHtmlContent content)
        {
            var writer = new StringWriter();
            content.WriteTo(writer, HtmlEncoder.Default);
            return writer.ToString();
        }

        public static string GetString(this IHtmlContentBuilder content)
        {
            var writer = new StringWriter();
            content.WriteTo(writer, HtmlEncoder.Default);
            return writer.ToString();
        }
    }
}
