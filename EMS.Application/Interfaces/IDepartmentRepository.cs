using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMS.Domain.Entities;

namespace EMS.Application.Interfaces
{
    public interface IDepartmentRepository
    {
        Task<Department?> GetByIdAsync(int id);             // May return null if department not found
        Task<IEnumerable<Department>> GetAllAsync();        // Returns a list of all departments.
        Task<Department> AddAsync(Department department);   // Adds a new department and returns the added department with its generated Id.
    }
}