using System.ComponentModel.DataAnnotations;

namespace StudentProjectManagement.Areas.Admin.Models
{
    public class Task
    {
        public int? TaskId { get; set; }

        public int ProjectId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Priority { get; set; }

        [Required]
        public int? AssignedTo { get; set; }

        public DateOnly Deadline { get; set; }

        public string Status { get; set; }

        public int? CreatedBy { get; set; }

        public int? LastModifiedBy { get; set; }

    }
}
