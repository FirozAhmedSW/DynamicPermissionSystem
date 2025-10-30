using Microsoft.AspNetCore.Mvc;

namespace DynamicPermissionSystem.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
