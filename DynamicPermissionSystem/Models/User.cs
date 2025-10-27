using System.Data;

namespace DynamicPermissionSystem.Models
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!; // demo only
        public int RoleId { get; set; }
        public Role? Role { get; set; }
    }
}
