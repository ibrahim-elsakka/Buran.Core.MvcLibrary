using Buran.Core.Library.LogUtil;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Buran.Core.MvcLibrary.LogUtil
{
    public class MvcLogger: Logger
    {
        public static string GetErrorMessage(ModelStateDictionary modelState)
        {
            var r = string.Empty;
            foreach (var state in modelState.Values)
            {
                foreach (var error in state.Errors)
                {
                    if (!string.IsNullOrWhiteSpace(r))
                        r += ", ";
                    if (string.IsNullOrWhiteSpace(error.ErrorMessage))
                    {
                        if (error.Exception != null)
                            r += error.Exception.Message;
                    }
                    else
                        r += error.ErrorMessage;
                }
            }
            if (!modelState.IsValid && string.IsNullOrWhiteSpace(r))
                r = "Unknown error!";
            r = r.Replace("\"", "").Replace("'", "");
            return r;
        }
    }
}
