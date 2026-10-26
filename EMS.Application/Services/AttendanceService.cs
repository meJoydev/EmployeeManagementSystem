using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMS.Application.DTOs.Attendance;
using EMS.Application.Interfaces;
using EMS.Domain.Entities;
using EMS.Domain.Enums;

namespace EMS.Application.Services
{
    public class AttendanceService
    {
        private readonly IAttendanceRepository _attendanceRepository;

        public AttendanceService(IAttendanceRepository attendanceRepository)
        {
            _attendanceRepository = attendanceRepository;
        }

        public async Task MarkAttendanceAsync(MarkAttendanceDto dto)
        {
            if (!Enum.TryParse<AttendanceStatus>(dto.Status, true, out var status))                             // Check(string-to-enum conversion) if dto.Status(Plain text) is a valid AttendanceStatus enum value, ignoring case
                throw new ArgumentException($"Invalid attendance status: {dto.Status}");

            var existing = await _attendanceRepository.GetByEmployeeAndDateAsync(dto.EmployeeId, dto.Date);     // Check if there's already an attendance record for this employee on this date

            if (existing is not null)
            {
                existing.UpdateStatus(status);                                                                  // if status mistakenly marked absent, update the existing attendance record with the new status
                await _attendanceRepository.UpdateAsync(existing);                                              // Save the updated attendance record to the repository
            }
            else
            {
                var attendance = new Attendance(dto.EmployeeId, dto.Date, status);                              // Create a new Attendance entity with the provided employee ID, date, and status
                await _attendanceRepository.AddAsync(attendance);                                               // Add the new attendance record to the repository
            }
        }
        public async Task<IEnumerable<AttendanceResponseDto>> GetByEmployeeAsync(int employeeId)                // Method to get all attendance records for a specific employee by their ID
        {                                                                                                       // Used IEnumerable as return type because it just means "a collection of things you can loop through"
            var records = await _attendanceRepository.GetByEmployeeIdAsync(employeeId);                         // Wait till it Fetches all attendance records for the specified employee from the repository GetByEmployeeIdAsync is the method that also sorts newest-first.
            return records.Select(MapToResponseDto);                                                            // Maps each raw attendance record to a response DTO (as written in AttendanceResponseDto) and returns the collection 
        }

        public async Task<IEnumerable<AttendanceResponseDto>> GetByDateAsync(DateTime date)                     // Same shape, different filter, To Fetch everyone's attendance on this one specific date
        {
            var records = await _attendanceRepository.GetByDateAsync(date);
            return records.Select(MapToResponseDto);                                                            // Map and return the collection of attendance records for the specified date as AttendanceResponseDto objects
        }

        private static AttendanceResponseDto MapToResponseDto(Attendance attendance)                            // Final Shape of the data we want to send back to the client, instead of sending the raw Attendance entity which contains more information than needed.
        {
            return new AttendanceResponseDto
            {
                Id = attendance.Id,
                EmployeeName = attendance.Employee is not null
                    ? $"{attendance.Employee.FirstName} {attendance.Employee.LastName}"                         // A terniary expression to check if the Employee navigation property is not null, and if so, concatenate the first and last name. If it is null, return "Unknown".
                    : "Unknown",
                Date = attendance.Date,
                Status = attendance.Status.ToString()                                                           // converts the AttendanceStatus enum into readable text ("Present")
            };
        }
    }
}