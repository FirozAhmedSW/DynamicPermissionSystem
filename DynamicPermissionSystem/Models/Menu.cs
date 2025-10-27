namespace DynamicPermissionSystem.Models
{
    public class Menu
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Controller { get; set; } = null!;
        public string Action { get; set; } = null!;
        public int? ParentId { get; set; }
    }
}
