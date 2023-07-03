using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PustokBookStore.Areas.Manage.Controllers
{
    [Authorize(Roles = "Admin, SuperAdmin")]
    [Area("manage")]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
