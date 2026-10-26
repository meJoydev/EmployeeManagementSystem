using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Application.DTOs.Salary
{
    public class SalaryRecordResponseDto            // Direction : OUT (what we get back when retrieving salary records of a specific employee)
    {
        public int Id { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal BaseSalary { get; set; }
        public decimal Deductions { get; set; }
        public decimal NetSalary { get; set; }
        public DateTime GeneratedOn { get; set; }
    }
}