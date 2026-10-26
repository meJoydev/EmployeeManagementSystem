using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMS.Domain.Entities;

namespace EMS.Application.Interfaces
{
    public interface ISalaryRecordRepository
    {
        Task<IEnumerable<SalaryRecord>> GetByEmployeeIdAsync(int employeeId);       // Returns a list of salary records for a specific employee
        Task<IEnumerable<SalaryRecord>> GetByMonthYearAsync(int month, int year);   // Returns a list of salary records for a specific month and year
        Task<SalaryRecord> AddAsync(SalaryRecord record);                           // Adds a new salary record and returns the added record with its generated Id.
    }
}