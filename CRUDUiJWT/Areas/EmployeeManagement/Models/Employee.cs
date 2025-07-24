namespace CRUDUiJWT.Areas.EmployeeManagement.Models
{
    public class Employee
    {
            public int EmpId { get; set; } // 4-digit employee ID
            public string Name { get; set; } = string.Empty;
            public DateTime JoinDate { get; set; }
            public DateTime DOB { get; set; }
            public string Gender { get; set; } = string.Empty;
            public string Department { get; set; } = string.Empty;
            public string Designation { get; set; } = string.Empty;
            //public int UserId { get; set; } // FK to Users table
       
    }
}
