using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMS.Domain.Entities;

namespace EMS.Application.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<Employee?> GetByIdAsync(int id);           // May return null if employee not found
        Task<IEnumerable<Employee>> GetAllAsync();      // Returns a list of all employees.
        Task<Employee> AddAsync(Employee employee);     // Adds a new employee and returns the added employee with its generated Id.
        Task UpdateAsync(Employee employee);            // Updates an existing employee. Assumes the employee exists.
        Task DeleteAsync(int id);                       // Deletes an employee by Id. Assumes the employee exists.
        Task<IEnumerable<Employee>> AddRangeAsync(IEnumerable<Employee> employees);     // Adds multiple employees and returns the added employees with their generated Ids.
        Task<IEnumerable<Employee>> GetByIdsAsync(IEnumerable<int> ids);                // Returns a list of employees matching the provided Ids. If an Id does not match any employee, it is simply ignored.
        Task UpdateRangeAsync(IEnumerable<Employee> employees);                         // Updates multiple employees. Assumes all provided employees exist.
    }
}