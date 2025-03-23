using System;
using System.ComponentModel.DataAnnotations;

namespace StudentProjectManagement.Areas.Student.Models
{
    public partial class Faculty
    {
        public int? UserId { get; set; }

        public int? FacultyId { get; set; }

        [Required]
        public string Designation { get; set; }

        public virtual User? User { get; set; }
    }
}
