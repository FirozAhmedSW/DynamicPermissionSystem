using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DynamicPermissionSystem.Models;

namespace DynamicPermissionSystem.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
            return RedirectToAction("Login", "Account");

        return View();
    }

}
