using DynamicPermissionSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DynamicPermissionSystem.Controllers
{
    public class MenuController : Controller
    {
        private readonly ApplicationDbContext _db;
        public MenuController(ApplicationDbContext db) { _db = db; }

        // ✅ List all menus
        public IActionResult Index()
        {
            var menus = _db.Menus.ToList();
            return View(menus);
        }

        // ✅ Create menu (GET)
        public IActionResult Create()
        {
            ViewBag.Menus = _db.Menus.ToList();
            return View();
        }

        // ✅ Create menu (POST)
        [HttpPost]
        public IActionResult Create(Menu menu)
        {
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
            var menu = _db.Menus.Find(id);
            if (menu == null) return NotFound();
            ViewBag.Menus = _db.Menus.ToList();
            return View(menu);
        }

        // ✅ Edit menu (POST)
        [HttpPost]
        public IActionResult Edit(Menu menu)
        {
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
            var menu = _db.Menus.FirstOrDefault(m => m.Id == id);
            if (menu == null) return NotFound();
            return View(menu);
        }

        // ✅ Delete menu (GET confirmation)
        public IActionResult Delete(int id)
        {
            var menu = _db.Menus.Find(id);
            if (menu == null) return NotFound();
            return View(menu);
        }

        // ✅ Delete confirmed
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
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
