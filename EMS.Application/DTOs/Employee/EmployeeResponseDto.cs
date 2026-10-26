using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Application.DTOs.Employee
{
    public class EmployeeResponseDto                            // Direction : OUT (what we get from the API)
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public decimal Salary { get; set; }
        public bool IsActive { get; set; }
        public DateTime DateOfJoining { get; set; }
    }
}