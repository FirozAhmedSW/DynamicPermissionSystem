using Microsoft.AspNetCore.Mvc;
using DynamicPermissionSystem.Models;

namespace DynamicPermissionSystem.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
            return RedirectToAction("Login", "Account");

        // Fetch dynamic counts
        ViewBag.TotalMenus = _context.Menus.Count();
        ViewBag.TotalUsers = _context.Users.Count();
        ViewBag.TotalRoles = _context.Roles.Count();
        ViewBag.TotalPermissions = _context.RoleMenuPermissions.Count();

        ViewBag.UserName = HttpContext.Session.GetString("UserName") ?? "User";

        return View();
    }
}
