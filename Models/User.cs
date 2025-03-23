using System.ComponentModel.DataAnnotations;

namespace StudentProjectManagement.Models
{
    public class User
    {
        public int? UserID { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [StringLength(100)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [StringLength(200)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(300)]
        public string PasswordHash { get; set; }

        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; }

        [StringLength(100)]
        public string Department { get; set; }

        public string? EnrollmentNo { get; set; }

        public string? Designation { get; set; }
    }
}
