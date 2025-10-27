namespace DynamicPermissionSystem.Models
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public ICollection<RoleMenuPermission>? RoleMenuPermissions { get; set; }
    }
}
