namespace UserGroupManager.Web.Models
{
    public class PermissionSelectionViewModel
    {
        public int PermissionId { get; set; }
        public string PermissionName { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
    }
}
