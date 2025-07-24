using System.ComponentModel.DataAnnotations;

namespace CRUDUiJWT.Models
{
    public class UserProfileViewModel
    {
            public int EmpId { get; set; }
            public string Name { get; set; }

        [Required, DataType(DataType.Date)]
        public DateTime JoinDate { get; set; }
            public DateTime DOB { get; set; }
            public string Gender { get; set; }
            public string Department { get; set; }
            public string Designation { get; set; }
            public string Role { get; set; }
    }
}
