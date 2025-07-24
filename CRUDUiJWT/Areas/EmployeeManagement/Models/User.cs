namespace CRUDUiJWT.Areas.EmployeeManagement.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty; // Used only during login/signup
        public string PasswordHash { get; set; } = string.Empty; // Stored in DB
        public string Role { get; set; } = string.Empty;
    }
}
