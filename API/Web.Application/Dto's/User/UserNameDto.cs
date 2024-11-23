using System.ComponentModel.DataAnnotations;

namespace Web.Application.Dto_s.User
{
    public class UserNameDto
    {
        [Required]
        [StringLength(30)]
        [MinLength(1)]
        [RegularExpression(@"^\S.*\S$|^\S$")]
        public string Name { get; set; }
    }
}