using Microsoft.AspNetCore.Mvc;

namespace DynamicPermissionSystem.Controllers
{
    public class ReportController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
