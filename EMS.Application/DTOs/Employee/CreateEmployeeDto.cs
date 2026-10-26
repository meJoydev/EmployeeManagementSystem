using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Application.DTOs.Employee             // DTO : They act as a filter to transfer data, Their Input and output shapes differ
{
    public class CreateEmployeeDto                  // Direction : IN (What we write in request body before hitting an endpoint)
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int DepartmentId { get; set; }
        public decimal Salary { get; set; }
    }
}