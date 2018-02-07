using Microsoft.AspNetCore.Mvc;

namespace Buran.Core.MvcLibrary.Extenders
{
    public class JavascriptResult: ContentResult
    {
        public JavascriptResult(string script)
        {
            Content = $"{script}";
            ContentType = "application/javascript";
        }
    }
}
