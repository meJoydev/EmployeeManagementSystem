using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Application.DTOs.Department
{
    public class DepartmentResponseDto                      // Diretion : OUT
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int EmployeeCount { get; set; }
    }
}