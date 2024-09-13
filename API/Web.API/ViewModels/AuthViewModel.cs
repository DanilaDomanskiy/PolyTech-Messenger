using System.ComponentModel.DataAnnotations;

namespace Web.API.Models
{
    public class AuthViewModel
    {
        [Required]
        [EmailAddress]
        public string Login { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
