using System.ComponentModel.DataAnnotations;

namespace UserGroupManager.Api.DataTranferObjects
{
    public class LoginUserDTO
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, MinLength(6)]
        public string Password { get; set; }
    }
}
