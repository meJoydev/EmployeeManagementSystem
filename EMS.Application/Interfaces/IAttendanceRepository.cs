using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMS.Domain.Entities;

namespace EMS.Application.Interfaces
{
    public interface IAttendanceRepository
    {
        Task<Attendance?> GetByIdAsync(int id);                 // May return null if attendance record not found
        Task<IEnumerable<Attendance>> GetByEmployeeIdAsync(int employeeId);     // Returns a list of attendance records for a specific employee
        Task<IEnumerable<Attendance>> GetByDateAsync(DateTime date);            // Returns a list of employees who were present/absent today
        Task<Attendance?> GetByEmployeeAndDateAsync(int employeeId, DateTime date);     // Check for duplicates before marking attendance
        Task<Attendance> AddAsync(Attendance attendance);       // Adds a new attendance record and returns the added record with its generated Id.
        Task UpdateAsync(Attendance attendance);                // Updates an existing attendance record. Assumes the record exists.
    }
}