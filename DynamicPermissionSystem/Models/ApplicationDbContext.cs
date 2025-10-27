using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace DynamicPermissionSystem.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }


        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<Menu> Menus => Set<Menu>();
        public DbSet<RoleMenuPermission> RoleMenuPermissions => Set<RoleMenuPermission>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            // seed basic data for demo
            modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "Admin" },
            new Role { Id = 2, Name = "Operator" }
            );


            modelBuilder.Entity<User>().HasData(
            new User { Id = 1, UserName = "admin", Password = "123", RoleId = 1 },
            new User { Id = 2, UserName = "operator", Password = "123", RoleId = 2 }
            );


            modelBuilder.Entity<Menu>().HasData(
            new Menu { Id = 1, Name = "Dashboard", Controller = "Home", Action = "Index", ParentId = null },
            new Menu { Id = 2, Name = "Person List", Controller = "Person", Action = "Index", ParentId = null },
            new Menu { Id = 3, Name = "Add Person", Controller = "Person", Action = "Create", ParentId = 2 },
            new Menu { Id = 4, Name = "Permissions", Controller = "Admin", Action = "ManagePermissions", ParentId = null }
            );


            // admin: all permissions
            modelBuilder.Entity<RoleMenuPermission>().HasData(
            new RoleMenuPermission { Id = 1, RoleId = 1, MenuId = 1, CanView = true, CanCreate = true, CanEdit = true, CanDelete = true },
            new RoleMenuPermission { Id = 2, RoleId = 1, MenuId = 2, CanView = true, CanCreate = true, CanEdit = true, CanDelete = true },
            new RoleMenuPermission { Id = 3, RoleId = 1, MenuId = 3, CanView = true, CanCreate = true, CanEdit = true, CanDelete = true },
            new RoleMenuPermission { Id = 4, RoleId = 1, MenuId = 4, CanView = true, CanCreate = true, CanEdit = true, CanDelete = true },
            // operator: only dashboard & person list view
            new RoleMenuPermission { Id = 5, RoleId = 2, MenuId = 1, CanView = true, CanCreate = false, CanEdit = false, CanDelete = false },
            new RoleMenuPermission { Id = 6, RoleId = 2, MenuId = 2, CanView = true, CanCreate = false, CanEdit = false, CanDelete = false }
            );
        }
    }
}
