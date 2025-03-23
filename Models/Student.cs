namespace StudentProjectManagement.Models
{
    public class Student
    {
        public int? UserId { get; set; }

        public int StudentId { get; set; }

        public string? EnrollmentNo { get; set; }

        public virtual User? User { get; set; }
    }
}
