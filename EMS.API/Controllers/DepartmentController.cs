using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using EMS.Application.DTOs.Department;
using EMS.Application.Services;

namespace EMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]                                                                         // Marks this as an API controller at api/department, and requires a valid JWT token for every action
    public class DepartmentController : ControllerBase
    {
        private readonly DepartmentService _departmentService;
        private readonly IValidator<CreateDepartmentDto> _createValidator;

        public DepartmentController(
            DepartmentService departmentService,
            IValidator<CreateDepartmentDto> createValidator)                            // Dependency injection for the department service and the validator for creating departments
        {
            _departmentService = departmentService;
            _createValidator = createValidator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()                                       // Action to get all departments, returns a list of departments in JSON format
        {
            var departments = await _departmentService.GetAllAsync();
            return Ok(departments);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)                                // Action to get a specific department by ID, returns the department in JSON format or a 404 if not found
        {
            var department = await _departmentService.GetByIdAsync(id);
            if (department is null)
                return NotFound(new { message = $"Department with ID {id} not found." });

            return Ok(department);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateDepartmentDto dto)                // Action to create a new department, validates the input DTO and returns the created department or validation errors
        {
            var validationResult = await _createValidator.ValidateAsync(dto);           // Validates the incoming DTO using FluentValidation
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var created = await _departmentService.CreateAsync(dto);                    // Calls the service to create the department 
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);  // Returns a 201 Created response with the location of the new resource
        }                                                                               // CreatedAtAction works here because a GetById endpoint exists for the Location header
    }
}