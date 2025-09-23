using System.ComponentModel.DataAnnotations;

namespace UserGroupManager.Api.DataTranferObjects
{
    public class RegisterUserDTO
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required,MinLength(6)]
        public string Password { get; set; }
    }
}
