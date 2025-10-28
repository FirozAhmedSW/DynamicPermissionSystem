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
            var users = _db.Users.Include(u => u.Role).ToList();
            var selectedUserId = userId ?? users.FirstOrDefault()?.Id ?? 0;

            var selectedUser = _db.Users.Find(selectedUserId);
            var roleId = selectedUser?.RoleId ?? 0;

            var menus = _db.Menus.ToList();
            var perms = _db.RoleMenuPermissions
                .Where(rp => rp.RoleId == roleId)
                .ToList();

            ViewBag.Users = users;
            ViewBag.SelectedUserId = selectedUserId;
            ViewBag.Menus = menus;
            ViewBag.Perms = perms;
            ViewBag.SelectedUser = userId;

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
