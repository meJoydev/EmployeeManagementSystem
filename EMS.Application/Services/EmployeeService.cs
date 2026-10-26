using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMS.Application.DTOs.Employee;
using EMS.Application.Interfaces;
using EMS.Domain.Entities;
using EMS.Application.Exceptions;

namespace EMS.Application.Services
{
    public class EmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeService(IEmployeeRepository employeeRepository)          // Constructor injection
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<EmployeeResponseDto> CreateEmployeeAsync(CreateEmployeeDto dto)                           // Method to create a new employee 
        {
            var employee = new Employee(dto.FirstName, dto.LastName, dto.Email, dto.DepartmentId, dto.Salary);      // Here we're converting the raw form data into a proper, rule-following Employee object (by handing it to the constructor).
            var created = await _employeeRepository.AddAsync(employee);         // Saves the employee to the database and returns the created employee object (with its new ID).
            return MapToResponseDto(created);                                   // Maps the created employee object to a response DTO and returns it.
        }

        public async Task<EmployeeResponseDto?> GetByIdAsync(int id)            // Method to get an employee by ID, might return null if not found
        {
            var employee = await _employeeRepository.GetByIdAsync(id);          // Fetches the employee from the database by ID
            return employee is null ? null : MapToResponseDto(employee);        // returns either null or the mapped response DTO
        }

        public async Task<IEnumerable<EmployeeResponseDto>> GetAllAsync()       // Method to get all employees, IEnumerable returns a collection of EmployeeResponseDto
        {
            var employees = await _employeeRepository.GetAllAsync();            // Fetches all employees from the database
            return employees.Select(MapToResponseDto);                          // Maps each raw employee data to a response DTO as written inside EmployeeResponseDto and returns the collection
        }

        public async Task<EmployeeResponseDto?> UpdateEmployeeAsync(int id, UpdateEmployeeDto dto)                  // Method to update an existing employee, might return null if not found
        {
            var employee = await _employeeRepository.GetByIdAsync(id);          // Look up the existing employee first. You can only update something that exists
            if (employee is null)
                return null;

            employee.UpdateDetails(dto.FirstName, dto.LastName, dto.Email, dto.DepartmentId);                       // Update the employee's details using the provided DTO. This ensures that the employee object is updated according to the business rules defined in the Employee entity.
            employee.UpdateSalary(dto.Salary);                                  // apply the new salary through its own existing method.
            await _employeeRepository.UpdateAsync(employee);                    // Save the updated employee back to the database

            var updated = await _employeeRepository.GetByIdAsync(id);           // Fetch the updated employee from the database to ensure we have the latest data
            return updated is null ? null : MapToResponseDto(updated);          // Map the updated employee to a response DTO and return it
        }

        public async Task<bool> DeleteEmployeeAsync(int id)                     // Method to delete an employee by ID, returns true if deleted, false if not found
        {
            var employee = await _employeeRepository.GetByIdAsync(id);          // Check if the employee exists before attempting to delete it.
            if (employee is null)
                return false;

            await _employeeRepository.DeleteAsync(id);                          // Soft deletes the emp data, It calls Deactivate() and the row stays in the database.
            return true;
        }

        public async Task<IEnumerable<EmployeeResponseDto>> BulkCreateAsync(BulkCreateEmployeesDto dto)             // Method to create multiple employees at once, returns a collection of EmployeeResponseDto
        {
            var emails = dto.Employees.Select(e => e.Email.ToLowerInvariant()).ToList();                            // Extracts the emails from the DTO and converts them to lowercase for case-insensitive comparison (because MySQL treats "AA.com" and "aa.com" as the same value under our unique index.)

            var duplicatesInRequest = emails
                .GroupBy(email => email)                                        // It groups equal emails together and keeps only groups with more than one member. It’s like sorting name tags into piles and flagging any pile with two or more.
                .Where(group => group.Count() > 1)
                .Select(group => group.Key)
                .ToList();                                                      

            if (duplicatesInRequest.Any())                                      // If the same email appears twice in one request, stop immediately with a clear message. The global handler turns it into 409.
                throw new ConflictException(
                    $"The request contains duplicate emails: {string.Join(", ", duplicatesInRequest)}");

            var existingEmployees = await _employeeRepository.GetAllAsync();    // Fetches all existing employees from the database to check for email conflicts. This is necessary because we need to ensure that none of the new emails already exist in the system.
            var existingEmails = existingEmployees
                .Select(e => e.Email.ToLowerInvariant())
                .ToHashSet();                                                   // Converts the existing emails to lowercase and stores them in a HashSet for efficient lookups. This allows us to quickly check if any of the new emails are already in use.

            var alreadyUsed = emails.Where(email => existingEmails.Contains(email)).ToList();                       // Checks if any of the new emails are already in use by comparing them against the existing emails. If any are found, they are collected into a list.

            if (alreadyUsed.Any())                                              // If any of the new emails are already in use, throw a ConflictException with a message listing the conflicting emails. The global handler will turn this into a 409 response.
                throw new ConflictException(
                    $"These emails already exist: {string.Join(", ", alreadyUsed)}");

            var employees = dto.Employees                                       // converts each DTO into a real entity using the constructor, so the rules (like auto-stamping DateOfJoining) apply to every one.
                .Select(e => new Employee(e.FirstName, e.LastName, e.Email, e.DepartmentId, e.Salary))
                .ToList();

            var created = await _employeeRepository.AddRangeAsync(employees);   // Saves all the new employees to the database in one go and returns the created employee objects (with their new IDs).

            var createdIds = created.Select(e => e.Id).ToList();                // Extracts the IDs of the newly created employees to use for re-fetching them with their departments loaded because the freshly created objects don’t have the Department loaded. The same re-fetch trick as in UpdateEmployeeAsync.
            var loaded = await _employeeRepository.GetByIdsAsync(createdIds);   // Fetches the newly created employees again, this time including their department information. This is necessary because the initial creation does not include department data, and we want to return complete information to the client.

            return loaded.Select(MapToResponseDto);
        }

        public async Task<BulkDeleteResultDto> BulkDeleteAsync(BulkDeleteEmployeesDto dto)
        {
            var requestedIds = dto.Ids.Distinct().ToList();                     // Removes repeated IDs, so [4, 4, 5] isn’t processed twice.

            var employees = (await _employeeRepository.GetByIdsAsync(requestedIds)).ToList();                       // fetches only the employees that exist among those IDs.

            foreach (var employee in employees)                                 // Soft deletes each employee by calling the Deactivate method
                employee.Deactivate();

            await _employeeRepository.UpdateRangeAsync(employees);              // saves all the changes in one transaction

            var foundIds = employees.Select(e => e.Id).ToHashSet();             // collects the IDs of the employees that were found and deleted, so we can report which ones weren’t found.
            var notFoundIds = requestedIds.Where(id => !foundIds.Contains(id)).ToList();                            // collects the IDs that were requested but not found in the database, so we can report them back to the client.

            return new BulkDeleteResultDto                                      // returns a DTO containing the count of deleted employees and the list of IDs that were not found
            {
                DeletedCount = employees.Count,
                NotFoundIds = notFoundIds
            };
        }

        private static EmployeeResponseDto MapToResponseDto(Employee employee)  // Maps an Employee entity to an EmployeeResponseDto(Filtered Emp data for client)
        {
            return new EmployeeResponseDto                                      // Converts and returns the real employee object into a client ready dto
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                DepartmentName = employee.Department?.Name ?? "Unassigned",     // employee.Department?.Name : where ? means If department is missing/null, Show null instead of crashing orelse Show Department name. ?? "Unassigned" means if its null then show Unassigned.
                Salary = employee.Salary,
                IsActive = employee.IsActive,
                DateOfJoining = employee.DateOfJoining
            };
        }
    }
}