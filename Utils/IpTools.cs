using Microsoft.AspNetCore.Http;

namespace Buran.Core.MvcLibrary.Utils
{
    public class IpTools
    {
        public string GetIp(HttpRequest request)
        {
            var ip = request.HttpContext.Connection.RemoteIpAddress;
            return ip.ToString();
        }

        public string GetAgent(HttpRequest request)
        {
            var agent = request.Headers["User-Agent"].ToString();
            return agent;
        }
    }
}
