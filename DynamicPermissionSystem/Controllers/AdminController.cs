using DynamicPermissionSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DynamicPermissionSystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _db;
        public AdminController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult ManagePermissions(int? roleId)
        {
            // 🔐 শুধুমাত্র Admin role access করতে পারবে
            var myRoleId = HttpContext.Session.GetInt32("RoleId") ?? 0;
            var currentRole = _db.Roles.Find(myRoleId);
            if (currentRole == null || currentRole.Name != "Admin")
                return Forbid();

            // 🔹 সব Roles আনছি dropdown এর জন্য
            var roles = _db.Roles.ToList();
            if (!roles.Any())
                return View(); // যদি কোনো role না থাকে

            // 🔹 যদি roleId null হয় তাহলে প্রথম role ধরে নিচ্ছি
            var selectedRoleId = roleId ?? roles.First().Id;

            // 🔹 Menus এবং Permissions আনছি
            var menus = _db.Menus
                .OrderBy(m => m.ParentId)
                .ThenBy(m => m.Name)
                .ToList();

            var perms = _db.RoleMenuPermissions
                .Where(p => p.RoleId == selectedRoleId)
                .ToList();

            // 🔹 ViewBag এ পাঠানো
            ViewBag.Roles = roles;
            ViewBag.Menus = menus;
            ViewBag.Perms = perms;
            ViewBag.SelectedRoleId = selectedRoleId;

            return View();
        }


        [HttpPost]
        public IActionResult SavePermissions(int roleId, List<string> flags)
        {
            flags = flags ?? new List<string>(); // avoid null
            var existing = _db.RoleMenuPermissions.Where(r => r.RoleId == roleId).ToList();
            _db.RoleMenuPermissions.RemoveRange(existing);

            foreach (var m in _db.Menus.ToList())
            {
                _db.RoleMenuPermissions.Add(new RoleMenuPermission
                {
                    RoleId = roleId,
                    MenuId = m.Id,
                    CanView = flags.Contains($"view_{m.Id}"),
                    CanCreate = flags.Contains($"create_{m.Id}"),
                    CanEdit = flags.Contains($"edit_{m.Id}"),
                    CanDelete = flags.Contains($"delete_{m.Id}")
                });
            }

            _db.SaveChanges();
            return RedirectToAction("ManagePermissions", new { roleId });
        }



        // ✅ 2. CRUD SECTION

        // 🔹 List all RoleMenuPermissions
        public IActionResult Index()
        {
            var data = _db.RoleMenuPermissions
                .Include(r => r.Role)
                .Include(m => m.Menu)
                .ToList();
            return View(data);
        }

        // 🔹 Create GET
        public IActionResult Create()
        {
            ViewBag.Roles = _db.Roles.ToList();
            ViewBag.Menus = _db.Menus.ToList();
            return View();
        }

        // 🔹 Create POST
        [HttpPost]
        public IActionResult Create(RoleMenuPermission model)
        {
            if (ModelState.IsValid)
            {
                _db.RoleMenuPermissions.Add(model);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Roles = _db.Roles.ToList();
            ViewBag.Menus = _db.Menus.ToList();
            return View(model);
        }

        // 🔹 Edit GET
        public IActionResult Edit(int id)
        {
            var perm = _db.RoleMenuPermissions.Find(id);
            if (perm == null) return NotFound();

            ViewBag.Roles = _db.Roles.ToList();
            ViewBag.Menus = _db.Menus.ToList();
            return View(perm);
        }

        // 🔹 Edit POST
        [HttpPost]
        public IActionResult Edit(RoleMenuPermission model)
        {
            if (ModelState.IsValid)
            {
                _db.RoleMenuPermissions.Update(model);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Roles = _db.Roles.ToList();
            ViewBag.Menus = _db.Menus.ToList();
            return View(model);
        }

        // 🔹 Delete GET
        public IActionResult Delete(int id)
        {
            var perm = _db.RoleMenuPermissions
                .Include(r => r.Role)
                .Include(m => m.Menu)
                .FirstOrDefault(x => x.Id == id);

            if (perm == null) return NotFound();
            return View(perm);
        }

        // 🔹 Delete POST
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var perm = _db.RoleMenuPermissions.Find(id);
            if (perm != null)
            {
                _db.RoleMenuPermissions.Remove(perm);
                _db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
