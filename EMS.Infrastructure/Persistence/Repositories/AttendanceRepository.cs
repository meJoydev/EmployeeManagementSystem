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
    public class AttendanceRepository : IAttendanceRepository
    {
        private readonly EmsDbContext _context;

        public AttendanceRepository(EmsDbContext context)       // Dependency injection of the DbContext to interact with the database
        {
            _context = context;
        }

        public async Task<Attendance?> GetByIdAsync(int id)     // Helps to find one specific attendance row, by its own unique ID
        {
            return await _context.Attendances                   // Wait till i get the attendance record with the specified Id
                .Include(a => a.Employee)                       // Include the related Employee entity to avoid lazy loading issues
                .FirstOrDefaultAsync(a => a.Id == id);          // Filter record by AttendanceId and return the first matching record or null if not found
        }

        public async Task<IEnumerable<Attendance>> GetByEmployeeIdAsync(int employeeId)     // Give all attendance records for a specific employee
        {
            return await _context.Attendances
                .Include(a => a.Employee)                       // Load all related Employee data, EF Core doesn’t fetch related data unless you ask
                .Where(a => a.EmployeeId == employeeId)
                .OrderByDescending(a => a.Date)                 // Sort by most recent
                .ToListAsync();                                 // Returns a list of attendance records
        }

        public async Task<IEnumerable<Attendance>> GetByDateAsync(DateTime date)
        {
            return await _context.Attendances
                .Include(a => a.Employee)                       // Load all related Employee data
                .Where(a => a.Date == date.Date)                // filter by date
                .ToListAsync();
        }

        public async Task<Attendance?> GetByEmployeeAndDateAsync(int employeeId, DateTime date)     // Check for specific employee's attendance on a specific date
        {
            return await _context.Attendances
                .FirstOrDefaultAsync(a => a.EmployeeId == employeeId && a.Date == date.Date);
        }

        public async Task<Attendance> AddAsync(Attendance attendance)       // Add new attendance record
        {
            await _context.Attendances.AddAsync(attendance);                // AddAsync(attendance): Starts tracking the new attendance entity in memory 
            await _context.SaveChangesAsync();                              // Successfully saves the new attendance record to the database
            return attendance;
        }

        public async Task UpdateAsync(Attendance attendance)
        {
            _context.Attendances.Update(attendance);                        // Update(attendance): Marks the attendance entity as modified in the DbContext, indicating that it has been changed and needs to be updated in the database
            await _context.SaveChangesAsync();                              // Commit the changes
        }
    }
}