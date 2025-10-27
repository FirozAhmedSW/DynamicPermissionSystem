using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DynamicPermissionSystem.Models;

namespace DynamicPermissionSystem.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() => View();
}
