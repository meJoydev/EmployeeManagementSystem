using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMS.Application.DTOs.Department;
using EMS.Application.Interfaces;
using EMS.Domain.Entities;

namespace EMS.Application.Services
{
    public class DepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;

        public DepartmentService(IDepartmentRepository departmentRepository)            // Constructor injection
        {
            _departmentRepository = departmentRepository;
        }

        public async Task<IEnumerable<DepartmentResponseDto>> GetAllAsync()             // Method to get all departments, IEnumerable returns a collection of DepartmentResponseDto
        {
            var departments = await _departmentRepository.GetAllAsync();                // Fetches all departments from the database
            return departments.Select(MapToResponseDto);                                // Maps each raw department data to a response DTO as written inside DepartmentResponseDto and returns the collection
        }

        public async Task<DepartmentResponseDto?> GetByIdAsync(int id)                  // Method to get a department by ID, might return null if not found
        {
            var department = await _departmentRepository.GetByIdAsync(id);
            return department is null ? null : MapToResponseDto(department);
        }

        public async Task<DepartmentResponseDto> CreateAsync(CreateDepartmentDto dto)   // Method to create a new department
        {
            var department = new Department(dto.Name);                                  // Creates a new Department instance with the provided name
            var created = await _departmentRepository.AddAsync(department);             // Saves the department to the database and returns the created department object (with its new ID)
            return MapToResponseDto(created);                                           // Maps the created department object to a response DTO and returns it
        }

        private static DepartmentResponseDto MapToResponseDto(Department department)    // Maps a Department entity to a DepartmentResponseDto (Filtered Department data for client)
        {
            return new DepartmentResponseDto
            {
                Id = department.Id,
                Name = department.Name,
                EmployeeCount = department.Employees.Count
            };
        }
    }
}