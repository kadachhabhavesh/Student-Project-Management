using System.Text.Json.Serialization;

namespace StudentProjectManagement.Models
{
    public class Faculty
    {
        public int? UserId { get; set; }

        public int FacultyId { get; set; }

        public string? Designation { get; set; }

        public virtual User? User { get; set; }
    }
}


