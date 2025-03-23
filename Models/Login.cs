using System.ComponentModel.DataAnnotations;

namespace StudentProjectManagement.Models
{
    public class Login
    {
        [Required(ErrorMessage = "Email required")]
        [EmailAddress(ErrorMessage = "Enter valid email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password required")]
        [MinLength(8)]
        public string Password { get; set; }
    }
}
