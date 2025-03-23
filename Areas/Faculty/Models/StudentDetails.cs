using StudentProjectManagement.Areas.Admin.Models;

namespace StudentProjectManagement.Areas.Faculty.Models
{
    public class StudentDetails
    {
        public Evaluation evaluation { get; set; }
        public Task task { get; set; }
        public StudentEvaluation studentEvaluation { get; set; }
    }
}
