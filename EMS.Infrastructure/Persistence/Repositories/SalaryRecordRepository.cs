using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EMS.Application.Interfaces;
using EMS.Domain.Entities;
using EMS.Infrastructure.Persistence;

namespace EMS.Infrastructure.Persistence.Repositories
{
    public class SalaryRecordRepository : ISalaryRecordRepository
    {
        private readonly EmsDbContext _context;

        public SalaryRecordRepository(EmsDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SalaryRecord>> GetByEmployeeIdAsync(int employeeId)       // Returns a list of salary records for a specific employee
        {
            return await _context.SalaryRecords                                                 // wait till i get the salary records
                .Include(s => s.Employee)                                                       // Include the related Employee entity in the query since EF Core doesn’t fetch related data unless you ask
                .Where(s => s.EmployeeId == employeeId)
                .OrderByDescending(s => s.Year).ThenByDescending(s => s.Month)                  // sorted in descending order
                .ToListAsync();
        }

        public async Task<IEnumerable<SalaryRecord>> GetByMonthYearAsync(int month, int year)   // Returns a list of salary records for a specific month and year
        {
            return await _context.SalaryRecords
                .Include(s => s.Employee)
                .Where(s => s.Month == month && s.Year == year)
                .ToListAsync();
        }

        public async Task<SalaryRecord> AddAsync(SalaryRecord record)                           // Adds a new salary record and returns the added record with its generated Id.
        {
            await _context.SalaryRecords.AddAsync(record);
            await _context.SaveChangesAsync();
            return record;
        }
    }
}
