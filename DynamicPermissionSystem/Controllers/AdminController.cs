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

        public IActionResult ManagePermissions(int? userId)
        {
            // 🔐 শুধু Admin ই access করতে পারবে
            var myRoleId = HttpContext.Session.GetInt32("RoleId") ?? 0;
            var currentRole = _db.Roles.Find(myRoleId);
            if (currentRole == null || currentRole.Name != "Admin")
                return RedirectToAction("Index");

            // 🔹 সব Users আনছি dropdown এর জন্য
            var users = _db.Users.Include(u => u.Role).ToList();
            if (!users.Any())
                return View(); // যদি কোনো user না থাকে

            // 🔹 যদি userId null হয় তাহলে প্রথম user ধরে নিচ্ছি
            var selectedUserId = userId ?? users.First().Id;

            // 🔹 User-এর Role বের করছি
            var user = _db.Users.Include(u => u.Role).FirstOrDefault(u => u.Id == selectedUserId);
            if (user == null)
                return View();

            // 🔹 Role অনুযায়ী Permissions আনছি
            var menus = _db.Menus.OrderBy(m => m.ParentId).ThenBy(m => m.Name).ToList();
            var perms = _db.RoleMenuPermissions.Where(p => p.RoleId == user.RoleId).ToList();

            // 🔹 ViewBag এ পাঠানো
            ViewBag.Users = users;
            ViewBag.Menus = menus;
            ViewBag.Perms = perms;
            ViewBag.SelectedUserId = selectedUserId;
            ViewBag.SelectedUserRole = user.Role?.Name ?? "Unknown";
            ViewBag.SelectedUser = userId;

            return View();
        }



        [HttpPost]
        public IActionResult SavePermissions(int userId, List<string> flags)
        {
            flags = flags ?? new List<string>(); // Null check

            // 🔹 User থেকে তার Role বের করি
            var user = _db.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                TempData["Error"] = "Invalid User!";
                return RedirectToAction("ManagePermissions");
            }

            int roleId = user.RoleId;

            // 🔹 পুরনো permission গুলো মুছে ফেলি
            var existing = _db.RoleMenuPermissions.Where(r => r.RoleId == roleId).ToList();
            _db.RoleMenuPermissions.RemoveRange(existing);

            // 🔹 নতুন permission গুলো যোগ করি
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

            TempData["Success"] = "✅ Permissions saved successfully!";
            return RedirectToAction("ManagePermissions", new { userId });
        }




        // ✅ 2. CRUD SECTION

        // 🔹 List all RoleMenuPermissions
        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var data = _db.RoleMenuPermissions.Include(r => r.Role).Include(m => m.Menu).ToList();
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
