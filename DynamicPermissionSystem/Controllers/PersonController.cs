using DynamicPermissionSystem.Helpers;
using DynamicPermissionSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPermissionSystem.Controllers
{
    public class PersonController : Controller
    {
        private readonly ApplicationDbContext _db;
        public PersonController(ApplicationDbContext db) { _db = db; }


        public IActionResult Index()
        {
            if (!PermissionHelper.HasPermission(HttpContext, _db, "Person", "Index"))
                return Forbid();


            var list = new List<string> { "Firoz", "Rahim" }; // demo list
            return View(list);
        }


        public IActionResult Create()
        {
            if (!PermissionHelper.HasPermission(HttpContext, _db, "Person", "Create"))
                return Forbid();
            return View();
        }


        [HttpPost]
        public IActionResult Create(string name)
        {
            if (!PermissionHelper.HasPermission(HttpContext, _db, "Person", "Create"))
                return Forbid();


            // save logic here
            return RedirectToAction("Index");
        }
    }
}
