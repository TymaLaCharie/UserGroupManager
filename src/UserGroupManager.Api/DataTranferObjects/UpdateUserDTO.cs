using System.ComponentModel.DataAnnotations;

namespace UserGroupManager.Api.DataTranferObjects
{
    public class UpdateUserDTO
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }
    }
}
