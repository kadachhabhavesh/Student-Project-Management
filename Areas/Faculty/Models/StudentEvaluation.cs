using System.ComponentModel.DataAnnotations;

namespace StudentProjectManagement.Areas.Faculty.Models
{
    public class StudentEvaluation
    {
        public int? StudentEvaluationsId { get; set; }

        public int ProjectId { get; set; }

        [Required]
        public int FacultyId { get; set; }

        public double Score { get; set; }

        public string Feedback { get; set; }

        [Required]
        public int StudentId { get; set; }

        public DateOnly? EvaluatedAt { get; set; }

        //extra fields for display information
        public Student? student { get; set; }
    }
}
