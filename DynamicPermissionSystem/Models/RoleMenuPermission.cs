namespace DynamicPermissionSystem.Models
{
    public class RoleMenuPermission
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public int MenuId { get; set; }
        public bool CanView { get; set; }
        public bool CanCreate { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }


        public Role? Role { get; set; }
        public Menu? Menu { get; set; }
    }
}
