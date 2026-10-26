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
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly EmsDbContext _context;

        public DepartmentRepository(EmsDbContext context)               // Injecting context from the EmsDbContext
        {
            _context = context;
        }

        public async Task<Department?> GetByIdAsync(int id)             // check if the department exists in the database by its Id
        {
            return await _context.Departments                           // Fetch the department table 
                .Include(d => d.Employees)
                .FirstOrDefaultAsync(d => d.Id == id);                  // Return the first department that matches the Id or null if not found
        }

        public async Task<IEnumerable<Department>> GetAllAsync()
        {
            return await _context.Departments
                .Include(d => d.Employees)
                .ToListAsync();                                         // Return all departments as a list, including their associated employees
        }

        public async Task<Department> AddAsync(Department department)
        {
            await _context.Departments.AddAsync(department);            // Make changes
            await _context.SaveChangesAsync();                          // Commit changes
            return department;                                          // Return department object with sql generated Id
        }
    }
}