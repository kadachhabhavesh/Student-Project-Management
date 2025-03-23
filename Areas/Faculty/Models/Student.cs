using System.ComponentModel.DataAnnotations;

namespace StudentProjectManagement.Areas.Faculty.Models
{
    public class Student
    {
        public int? UserId { get; set; }

        public int? StudentId { get; set; }

        [Required]
        public string EnrollmentNo { get; set; }

        public virtual User User { get; set; }
    }
}
