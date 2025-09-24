using System.ComponentModel.DataAnnotations;

namespace UserGroupManager.Web.Models
{
    public class RegisterDTO
    {
        [Required] 
        public string FirstName { get; set; }
        [Required] 
        public string LastName { get; set; }
        [Required, EmailAddress] 
        public string Email { get; set; }
        [Required, MinLength(6)] 
        public string Password { get; set; }
    }
}
