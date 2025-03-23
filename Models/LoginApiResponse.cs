namespace StudentProjectManagement.Models
{
    public class LoginApiResponse
    {
        public int? UserId { get; set; }
        public string? UserRole { get; set; }
        public string Message { get; set; }
        public string Token { get; set; }
        public string UserName { get; set; }
    }
}
