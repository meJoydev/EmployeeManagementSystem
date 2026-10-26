using DocumentFormat.OpenXml.InkML;
using EMS.Application.Interfaces;
using EMS.Domain.Entities;
using EMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Infrastructure.Persistence.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly EmsDbContext _context;

        public EmployeeRepository(EmsDbContext context)     // Constructor Injection from DB Context
        {
            _context = context;
        }

        public async Task<Employee?> GetByIdAsync(int id)
        {
            return await _context.Employees                 // Wait till i get the employee data from the database
                .Include(e => e.Department)                 // .Include(...): Performs Eager Loading. It tells EF Core to generate a SQL LEFT JOIN or INNER JOIN.
                .FirstOrDefaultAsync(e => e.Id == id);      // e => e.Department: Specifies the navigation property to load. This ensures to load the related Department object inside the Employee model 
        }                                                   // Using FirstOrDefaultAsync to return the first matching employee or null if not found, e => e.Id == id: A lambda expression acting as a filter

        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            return await _context.Employees
                .Include(e => e.Department)
                .ToListAsync();                             // ToListAsync: Executes the query and returns the results as a List<Employee>
        }

        public async Task<Employee> AddAsync(Employee employee)
        {
            await _context.Employees.AddAsync(employee);    // .AddAsync(employee): Starts tracking the new employee entity in memory, this step does not send the data to the database yet.
            await _context.SaveChangesAsync();              // This is when a real INSERT command runs against MySQL and the employee gets a real, permanent Id
            return employee;                                // Return the employee object with its newly generated Id
        }                                                   
        public async Task UpdateAsync(Employee employee)
        {
            _context.Employees.Update(employee);            // .Update(employee): Marks the employee entity as modified
            await _context.SaveChangesAsync();              // Commit the changes to the database, generating an UPDATE SQL command
        }

        public async Task DeleteAsync(int id)               // Never deletes the employee from the database, instead it marks the employee as inactive
        {
            var employee = await _context.Employees.FindAsync(id);  // .FindAsync(id): Searches for the employee by its primary key (Id)
            if (employee is not null)
            {
                employee.Deactivate();                      // if found, Mark it as inactive by calling the Deactivate()
                await _context.SaveChangesAsync();          // Commit Changes
            }
        }

        public async Task<IEnumerable<Employee>> AddRangeAsync(IEnumerable<Employee> employees)
        {
            var list = employees.ToList();                  // Convert the IEnumerable to a List so it isn’t re-evaluated later.
            await _context.Employees.AddRangeAsync(list);   // AddRangeAsync(list): Stages all the new employee entities for insertion into the database. This is more efficient than adding them one by one. Nothing touches MySQL yet, like writing several cheques before depositing any.
            await _context.SaveChangesAsync();              // Commit all the changes to the database in one go, generating a single batch INSERT command.
            return list;                                    // Return the list of employees with their newly generated Ids.
        }

        public async Task<IEnumerable<Employee>> GetByIdsAsync(IEnumerable<int> ids)
        {
            var idList = ids.ToList();                      // Makes a list that EF Core can translate into SQL
            return await _context.Employees
                .Include(e => e.Department)                 // loads the department, so names show in responses.
                .Where(e => idList.Contains(e.Id))          // It becomes SQL WHERE Id IN (1, 2, 3): “give me only these IDs”.
                .OrderBy(e => e.Id)
                .ToListAsync();
        }

        public async Task UpdateRangeAsync(IEnumerable<Employee> employees)
        {
            _context.Employees.UpdateRange(employees);      // The same as UpdateAsync, but for many objects, saved in one go.
            await _context.SaveChangesAsync();
        }
    }
}
// NOTE: In AddAsync(Employee employee) method, WHY TWO SEPERATE STEPS ?
//       To stage multiple related changes together, and only commit them all at once, as a single unit (a transaction) to prevent inconsistency.
//       Either both succeed together, or if something fails, neither gets saved at all.
// Eg:   await _context.Employees.AddAsync(newEmployee);
//       await _context.SalaryRecords.AddAsync(newSalaryRecord);
// If you want to add an employee and also create a related SalaryRecords record, you can stage both changes and then call SaveChangesAsync() once.
// If the SalaryRecords record fails to save, the employee won't be added either, maintaining data integrity.