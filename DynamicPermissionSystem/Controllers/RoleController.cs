using DynamicPermissionSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPermissionSystem.Controllers
{
    public class RoleController : Controller
    {
        private readonly ApplicationDbContext _db;
        public RoleController(ApplicationDbContext db)
        {
            _db = db;
        }

        // ✅ Role List
        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");
            var roles = _db.Roles.ToList();
            return View(roles);
        }

        // ✅ Create - GET
        public IActionResult Create()
        {
            return View();
        }

        // ✅ Create - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Role role)
        {
            if (ModelState.IsValid)
            {
                _db.Roles.Add(role);
                _db.SaveChanges();
                TempData["success"] = "Role created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(role);
        }

        // ✅ Edit - GET
        public IActionResult Edit(int id)
        {
            var role = _db.Roles.Find(id);
            if (role == null)
                return NotFound();

            return View(role);
        }

        // ✅ Edit - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Role role)
        {
            if (ModelState.IsValid)
            {
                _db.Roles.Update(role);
                _db.SaveChanges();
                TempData["success"] = "Role updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(role);
        }

        // ✅ Delete
        public IActionResult Delete(int id)
        {
            var role = _db.Roles.Find(id);
            if (role != null)
            {
                _db.Roles.Remove(role);
                _db.SaveChanges();
                TempData["success"] = "Role deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
