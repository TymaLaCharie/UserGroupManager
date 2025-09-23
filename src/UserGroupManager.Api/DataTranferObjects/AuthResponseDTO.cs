using System.ComponentModel.DataAnnotations;

namespace UserGroupManager.Api.DataTranferObjects
{
    public class AuthResponseDTO
    {
        public string Email { get; set; }
        public string AuthToken { get; set; }
    }
}
