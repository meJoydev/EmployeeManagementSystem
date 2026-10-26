using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMS.Application.DTOs.Salary;
using EMS.Application.Interfaces;
using EMS.Domain.Entities;
using EMS.Domain.Enums;

namespace EMS.Application.Services
{
    public class SalaryService
    {
        private const int WorkingDaysPerMonth = 30;                                                         // A named constant for our assumption

        private readonly IEmployeeRepository _employeeRepository;
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly ISalaryRecordRepository _salaryRecordRepository;

        public SalaryService(
            IEmployeeRepository employeeRepository,
            IAttendanceRepository attendanceRepository,
            ISalaryRecordRepository salaryRecordRepository)                                                 // Constructor injection for the repositories
        {
            _employeeRepository = employeeRepository;
            _attendanceRepository = attendanceRepository;
            _salaryRecordRepository = salaryRecordRepository;
        }

        public async Task<int> GenerateMonthlySalaryAsync(int month, int year)                              // Method to generate monthly salary records for all active employees, It returns only the no. of records created, not the actual records themselves since it creates many records in one go and the caller doesn't need to know the details of each record, just how many were created.
        {
            if (month < 1 || month > 12)
                throw new ArgumentException("Month must be between 1 and 12.");
            if (year < 2000 || year > DateTime.UtcNow.Year + 1)
                throw new ArgumentException("Year is not valid.");

            var allEmployees = await _employeeRepository.GetAllAsync();                                     // Fetch all employees from the repository
            var activeEmployees = allEmployees.Where(e => e.IsActive).ToList();                             // Filter to only include active employees

            var existingRecords = await _salaryRecordRepository.GetByMonthYearAsync(month, year);           // Fetch existing salary records for the specified month and year
            var alreadyGenerated = existingRecords.Select(r => r.EmployeeId).ToHashSet();                   // Create a HashSet of employee IDs for whom salary records already exist. Hashset is used here for preventing duplicate salary records for the same month and year 

            int createdCount = 0;

            foreach (var employee in activeEmployees)                                                       // Process one employee at a time
            {
                if (alreadyGenerated.Contains(employee.Id))                                                 // If a salary record already exists for this employee for the specified month and year, skip to the next employee
                    continue;

                var attendance = await _attendanceRepository.GetByEmployeeIdAsync(employee.Id);             // Fetch attendance records for the employee
                var monthRecords = attendance
                    .Where(a => a.Date.Month == month && a.Date.Year == year)
                    .ToList();                                                                              // Filter attendance records to only include those for the specified month and year

                decimal absentDays = monthRecords.Count(a => a.Status == AttendanceStatus.Absent);          // Count the number of days the employee was absent
                decimal halfDays = monthRecords.Count(a => a.Status == AttendanceStatus.HalfDay);           // Count the number of half days the employee was present

                decimal perDaySalary = employee.Salary / WorkingDaysPerMonth;                               // Calculate the per-day salary based on the employee's monthly salary and our assumed working days per month
                decimal deductions = Math.Round(perDaySalary * (absentDays + halfDays * 0.5m), 2);          // Calculate deductions based on absent and half days. The m after 0.5 marks it as a decimal literal. Math.Round(..., 2) keeps two decimal places, like paise.
                deductions = Math.Min(deductions, employee.Salary);                                         // Ensure that deductions do not exceed the employee's salary and push net pay negative

                var record = new SalaryRecord(employee.Id, month, year, employee.Salary, deductions);       // Create a new salary record for the employee with the calculated deductions
                await _salaryRecordRepository.AddAsync(record);                                             // Add the new salary record to the repository
                createdCount++;                                                                             // Increment the count of created records
            }

            return createdCount;                                                                            // Since the caller needs to know how many records were created, Count helps to differenciate between a successful operation and a failed one.
        }

        public async Task<IEnumerable<SalaryRecordResponseDto>> GetByMonthYearAsync(int month, int year)    // Method to retrieve salary records for a specific month and year, and returning them as SalaryRecordResponseDto
        {
            var records = await _salaryRecordRepository.GetByMonthYearAsync(month, year);                   // Fetch salary records for the specified month and year from the repository
            return records.Select(MapToResponseDto);                                                        // Map each SalaryRecord to a SalaryRecordResponseDto using the MapToResponseDto method
        }

        public async Task<IEnumerable<SalaryRecordResponseDto>> GetByEmployeeAsync(int employeeId)          // Method to retrieve salary records for a specific employee
        {
            var records = await _salaryRecordRepository.GetByEmployeeIdAsync(employeeId);                   // Fetch salary records for the specified employee 
            return records.Select(MapToResponseDto);                                                        // Map it
        }

        private static SalaryRecordResponseDto MapToResponseDto(SalaryRecord record)                        // Private helper method to map a SalaryRecord entity to a SalaryRecordResponseDto 
        {
            return new SalaryRecordResponseDto                                                              // Final shape of the data that will be sent to the client 
            {
                Id = record.Id,
                EmployeeName = record.Employee is not null
                    ? $"{record.Employee.FirstName} {record.Employee.LastName}"
                    : "Unknown",                                                                            // If the Employee navigation property is null, we return "Unknown" as the employee name. This can happen if the employee was deleted after the salary record was created.
                Month = record.Month,
                Year = record.Year,
                BaseSalary = record.BaseSalary,
                Deductions = record.Deductions,
                NetSalary = record.NetSalary,
                GeneratedOn = record.GeneratedOn
            };
        }
    }
}