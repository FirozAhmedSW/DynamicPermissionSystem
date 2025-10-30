using DynamicPermissionSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DynamicPermissionSystem.Controllers
{
    public class MenuController : Controller
    {
        private readonly ApplicationDbContext _db;
        public MenuController(ApplicationDbContext db)
        {
            _db = db;
        }

        // 🔹 Helper Method: Check Permission Dynamically
        private bool HasPermission(string actionType)
        {
            var roleId = HttpContext.Session.GetInt32("RoleId");
            if (roleId == null) return false;

            // Find the Menu corresponding to this controller & Index action
            var menu = _db.Menus.FirstOrDefault(m => m.Controller == "Menu" && m.Action == "Index");
            if (menu == null) return false;

            var permission = _db.RoleMenuPermissions.FirstOrDefault(rp => rp.RoleId == roleId && rp.MenuId == menu.Id);
            if (permission == null) return false;

            return actionType switch
            {
                "View" => permission.CanView == true,
                "Create" => permission.CanCreate == true,
                "Edit" => permission.CanEdit == true,
                "Delete" => permission.CanDelete == true,
                _ => false
            };
        }

        // ✅ List all menus
        public IActionResult Index()
        {
            if (!HasPermission("View"))
                return RedirectToAction("AccessDenied", "Auth");

            var menus = _db.Menus.ToList();
            return View(menus);
        }

        // ✅ Create menu (GET)
        public IActionResult Create()
        {
            if (!HasPermission("Create"))
                return RedirectToAction("AccessDenied", "Auth");

            ViewBag.Menus = _db.Menus.ToList();
            return View();
        }

        // ✅ Create menu (POST)
        [HttpPost]
        public IActionResult Create(Menu menu)
        {
            if (!HasPermission("Create"))
                return RedirectToAction("AccessDenied", "Auth");

            if (ModelState.IsValid)
            {
                _db.Menus.Add(menu);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Menus = _db.Menus.ToList();
            return View(menu);
        }

        // ✅ Edit menu (GET)
        public IActionResult Edit(int id)
        {
            if (!HasPermission("Edit"))
                return RedirectToAction("AccessDenied", "Auth");

            var menu = _db.Menus.Find(id);
            if (menu == null) return NotFound();

            ViewBag.Menus = _db.Menus.ToList();
            return View(menu);
        }

        // ✅ Edit menu (POST)
        [HttpPost]
        public IActionResult Edit(Menu menu)
        {
            if (!HasPermission("Edit"))
                return RedirectToAction("AccessDenied", "Auth");

            if (ModelState.IsValid)
            {
                _db.Menus.Update(menu);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Menus = _db.Menus.ToList();
            return View(menu);
        }

        // ✅ Details menu
        public IActionResult Details(int id)
        {
            if (!HasPermission("View"))
                return RedirectToAction("AccessDenied", "Auth");

            var menu = _db.Menus.FirstOrDefault(m => m.Id == id);
            if (menu == null) return NotFound();
            return View(menu);
        }

        // ✅ Delete menu (GET confirmation)
        public IActionResult Delete(int id)
        {
            if (!HasPermission("Delete"))
                return RedirectToAction("AccessDenied", "Auth");

            var menu = _db.Menus.Find(id);
            if (menu == null) return NotFound();
            return View(menu);
        }

        // ✅ Delete confirmed
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            if (!HasPermission("Delete"))
                return RedirectToAction("AccessDenied", "Auth");

            var menu = _db.Menus.Find(id);
            if (menu != null)
            {
                _db.Menus.Remove(menu);
                _db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
