using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Web.API.ViewModels.UserVIewModels
{
    public class RegisterViewModel
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [PasswordPropertyText]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }
    }
}
