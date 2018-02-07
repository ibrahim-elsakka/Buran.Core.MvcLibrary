using Microsoft.AspNetCore.Routing;

namespace Buran.Core.MvcLibrary.Grid
{
    public class LibGeneral
    {
        public static string GetContentUrl(RouteData route)
        {
            var root = string.Empty;
            if (route.DataTokens["area"] != null)
                root = route.DataTokens["area"].ToString();
            else if (route.Values["area"] != null)
                root = route.Values["area"].ToString();

            if (route.Values["controller"] != null)
            {
                if (!string.IsNullOrWhiteSpace(root))
                    root += "/";
                root += (string)route.Values["controller"];
            }
            return root;
        }
    }
}
