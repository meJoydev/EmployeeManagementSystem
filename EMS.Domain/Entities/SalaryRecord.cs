using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Domain.Entities
{
    public class SalaryRecord
    {
        public int Id { get; private set; }
        public int EmployeeId { get; private set; }                 // Foreign Key to Employee table
        public Employee Employee { get; private set; } = null!;     // we can directly access employee table details (salaryRecord.Employee.FirstName) without needing to query the Employee table separately
        public int Month { get; private set; }
        public int Year { get; private set; }
        public decimal BaseSalary { get; private set; }             // decimal avoids rounding errors that can occur with floating-point types, which is crucial for financial calculations
        public decimal Deductions { get; private set; }
        public decimal NetSalary { get; private set; }
        public DateTime GeneratedOn { get; private set; }

        private SalaryRecord() { }                                  // EF Core needs a parameterless constructor to rebuild objects from the database.
        public SalaryRecord(int employeeId, int month, int year, decimal baseSalary, decimal deductions)
        {
            EmployeeId = employeeId;
            Month = month;
            Year = year;
            BaseSalary = baseSalary;
            Deductions = deductions;
            NetSalary = baseSalary - deductions;
            GeneratedOn = DateTime.UtcNow;
        }
    }
}
