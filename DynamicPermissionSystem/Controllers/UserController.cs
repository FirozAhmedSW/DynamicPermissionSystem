using DynamicPermissionSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DynamicPermissionSystem.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;
        public UserController(ApplicationDbContext db)
        {
            _db = db;
        }

        // ✅ Helper Method: Check Permission Dynamically
        private bool HasPermission(string actionType)
        {
            var roleId = HttpContext.Session.GetInt32("RoleId");
            if (roleId == null) return false;

            // Find the Menu corresponding to this controller & Index action
            var menu = _db.Menus.FirstOrDefault(m => m.Controller == "User" && m.Action == "Index");
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

        // 🧩 User List
        public IActionResult Index()
        {
            if (!HasPermission("View"))
                return RedirectToAction("AccessDenied", "Auth");

            var users = _db.Users.Include(u => u.Role).ToList();
            return View(users);
        }

        // 🧩 Create (GET)
        public IActionResult Create()
        {
            if (!HasPermission("Create"))
                return RedirectToAction("AccessDenied", "Auth");

            ViewBag.Roles = _db.Roles.ToList();
            return View();
        }

        // 🧩 Create (POST)
        [HttpPost]
        public IActionResult Create(User user)
        {
            if (!HasPermission("Create"))
                return RedirectToAction("AccessDenied", "Auth");

            if (ModelState.IsValid)
            {
                _db.Users.Add(user);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Roles = _db.Roles.ToList();
            return View(user);
        }

        // 🧩 Edit (GET)
        public IActionResult Edit(int id)
        {
            if (!HasPermission("Edit"))
                return RedirectToAction("AccessDenied", "Auth");

            var user = _db.Users.Find(id);
            if (user == null) return NotFound();

            ViewBag.Roles = _db.Roles.ToList();
            return View(user);
        }

        // 🧩 Edit (POST)
        [HttpPost]
        public IActionResult Edit(User user)
        {
            if (!HasPermission("Edit"))
                return RedirectToAction("AccessDenied", "Auth");

            if (ModelState.IsValid)
            {
                _db.Users.Update(user);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Roles = _db.Roles.ToList();
            return View(user);
        }

        // 🧩 Delete
        public IActionResult Delete(int id)
        {
            if (!HasPermission("Delete"))
                return RedirectToAction("AccessDenied", "Auth");

            var user = _db.Users.Find(id);
            if (user != null)
            {
                _db.Users.Remove(user);
                _db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
