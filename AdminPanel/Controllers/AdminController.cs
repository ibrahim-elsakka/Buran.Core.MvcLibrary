using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Buran.Core.MvcLibrary.AdminPanel.Controllers
{
    [Authorize(Roles = "admin")]
    [Area("Panel")]
    public class AdminController : Controller
    {
        public AdminController()
        {

        }
    }
}
