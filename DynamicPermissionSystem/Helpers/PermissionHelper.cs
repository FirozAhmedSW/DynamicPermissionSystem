using DynamicPermissionSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace DynamicPermissionSystem.Helpers
{
    public static class PermissionHelper
    {
        public static bool HasPermission(HttpContext httpContext, ApplicationDbContext db, string controller, string action)
        {
            var roleId = httpContext.Session.GetInt32("RoleId") ?? 0;
            if (roleId == 0) return false;


            var perm = db.RoleMenuPermissions
            .Include(p => p.Menu)
            .FirstOrDefault(p => p.RoleId == roleId && p.Menu != null && p.Menu.Controller == controller && p.Menu.Action == action);


            return perm != null && perm.CanView;
        }
    }
}
