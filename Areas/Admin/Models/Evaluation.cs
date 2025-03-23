using System.ComponentModel.DataAnnotations;

namespace StudentProjectManagement.Areas.Admin.Models
{
    public class Evaluation
    {
        public int? EvaluationId { get; set; }

        public int ProjectId { get; set; }

        [Required]
        public int FacultyId { get; set; }

        public double Score { get; set; }

        public string Feedback { get; set; }

        public DateOnly? EvaluatedAt { get; set; }

        public virtual Faculty? Faculty { get; set; }

        public virtual Project? Project { get; set; }
    }
}
