using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using DynamicPermissionSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DynamicPermissionSystem.Controllers
{
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ReportController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");
            // 🔹 Get role from session
            var roleId = HttpContext.Session.GetInt32("RoleId") ?? 0;

            // 🔹 Check permission for this menu
            var hasPermission = _db.RoleMenuPermissions
                                   .Include(rp => rp.Menu)
                                   .Any(rp => rp.RoleId == roleId
                                           && rp.CanView
                                           && rp.Menu.Controller == "Report"
                                           && rp.Menu.Action == "Index");

            if (!hasPermission)
            {
                // Redirect to home or show "Access Denied"
                return RedirectToAction("AccessDenied", "Home");
            }

            return View();
        }
    }
}
