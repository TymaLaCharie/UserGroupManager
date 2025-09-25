using System.ComponentModel.DataAnnotations;

namespace UserGroupManager.Web.Models
{
    public class UpdateGroupDTO
    {
        [Required(ErrorMessage = "Group name is required")]
        public string Name { get; set; }
    }
}
