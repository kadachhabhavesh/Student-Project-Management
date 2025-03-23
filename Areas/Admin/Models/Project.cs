using System.ComponentModel.DataAnnotations;

namespace StudentProjectManagement.Areas.Admin.Models
{
    public class Project
    {
        public int ProjectId { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Start Date is required.")]
        [DataType(DataType.Date, ErrorMessage = "Invalid date format.")]
        public DateOnly StartDate { get; set; }

        [Required(ErrorMessage = "End Date is required.")]
        [DataType(DataType.Date, ErrorMessage = "Invalid date format.")]
        public DateOnly EndDate { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        [RegularExpression("^(Completed|Active|Pending)$", ErrorMessage = "Invalid status. Allowed values are Completed, Active, or Pending.")]
        public string Status { get; set; }

        [Required(ErrorMessage = "Mentor is required.")]
        public int? MentorId { get; set; }

        public virtual Faculty Mentor { get; set; }
    }
}
