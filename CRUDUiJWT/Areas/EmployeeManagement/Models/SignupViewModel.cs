namespace CRUDUiJWT.Areas.EmployeeManagement.Models
{
    public class SignupViewModel : User
    {
        public string Role { get; set; } = string.Empty;

        // Optional employee-specific fields
        public int? EmpId { get; set; }
        public string? Name { get; set; }
        public DateTime? JoinDate { get; set; }
        public DateTime? DOB { get; set; }
        public string? Gender { get; set; }
        public string? Department { get; set; }
        public string? Designation { get; set; }
    }
}
