using DynamicPermissionSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPermissionSystem.Controllers
{
    public class RoleController : Controller
    {
        private readonly ApplicationDbContext _db;
        public RoleController(ApplicationDbContext db) { _db = db; }

        public IActionResult Index() => View(_db.Roles.ToList());

        public IActionResult Create() => View();

        [HttpPost]
        public IActionResult Create(Role role)
        {
            if (ModelState.IsValid)
            {
                _db.Roles.Add(role);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(role);
        }

        public IActionResult Edit(int id)
        {
            var role = _db.Roles.Find(id);
            if (role == null) return NotFound();
            return View(role);
        }

        [HttpPost]
        public IActionResult Edit(Role role)
        {
            if (ModelState.IsValid)
            {
                _db.Roles.Update(role);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(role);
        }

        public IActionResult Delete(int id)
        {
            var role = _db.Roles.Find(id);
            if (role != null)
            {
                _db.Roles.Remove(role);
                _db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
