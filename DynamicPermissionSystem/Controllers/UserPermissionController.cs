using DynamicPermissionSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DynamicPermissionSystem.Controllers
{
    public class UserPermissionController : Controller
    {
        private readonly ApplicationDbContext _db;
        public UserPermissionController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: User-wise menu permissions
        public IActionResult Index(int? userId)
        {
            // 🔹 সব Users আনছি dropdown এর জন্য
            var users = _db.Users.Include(u => u.Role).ToList();

            if (!users.Any())
                return View(); // যদি কোনো user না থাকে, blank view দেখাবে

            // 🔹 যদি userId null হয় তাহলে প্রথম user ধরে নিচ্ছি
            var selectedUserId = userId ?? users.First().Id;

            // 🔹 User ও তার Role বের করছি
            var selectedUser = _db.Users.Include(u => u.Role).FirstOrDefault(u => u.Id == selectedUserId);

            if (selectedUser == null)
                return View();

            var roleId = selectedUser.RoleId;
            var selectedUserRole = selectedUser.Role?.Name ?? "N/A";

            // 🔹 Role অনুযায়ী permissions আনছি
            var menus = _db.Menus.OrderBy(m => m.ParentId).ThenBy(m => m.Name).ToList();

            var perms = _db.RoleMenuPermissions.Where(p => p.RoleId == roleId).ToList();

            // 🔹 ViewBag এ পাঠাচ্ছি
            ViewBag.Users = users;
            ViewBag.SelectedUserId = selectedUserId;
            ViewBag.Menus = menus;
            ViewBag.Perms = perms;
            ViewBag.SelectedUserRole = selectedUserRole;

            return View();
        }



        // POST: Save permissions for selected user
        [HttpPost]
        public IActionResult Save(int userId, List<int> menuIds)
        {
            var user = _db.Users.Find(userId);
            if (user == null) return NotFound();

            // Remove all existing permissions for this role
            var existing = _db.RoleMenuPermissions.Where(rp => rp.RoleId == user.RoleId).ToList();
            _db.RoleMenuPermissions.RemoveRange(existing);

            // Add new permissions
            foreach (var menuId in menuIds)
            {
                _db.RoleMenuPermissions.Add(new RoleMenuPermission
                {
                    RoleId = user.RoleId,
                    MenuId = menuId,
                    CanView = true
                });
            }

            _db.SaveChanges();
            return RedirectToAction("Index", new { userId });
        }
    }
}
