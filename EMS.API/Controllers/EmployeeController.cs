using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using EMS.Application.DTOs.Employee;
using EMS.Application.Services;

namespace EMS.API.Controllers
{
    [ApiController]                                                         // Its a metadata that tells ASP.NET Core "treat this class specially, as an API controller", and it automatically handle things like model validation errors, JSON formatting, and request binding in the standard API way.
    [Route("api/[controller]")]                                             // Route of the controller [api/employee]
    [Authorize]                                                             // This attribute indicates that all actions in this controller require the user to be authenticated. If a request is made to any action without a valid JWT authentication token, the request will be denied with a 401 Unauthorized response.
    public class EmployeeController : ControllerBase                        // Inherits from ControllerBase, which provides the basic functionality for handling HTTP requests, responses and helper methods like Ok(), NotFound(), BadRequest(), CreatedAtAction() etc.
    {
        private readonly EmployeeService _employeeService;                  // Stores the instance of the EmployeeService class
        private readonly IValidator<CreateEmployeeDto> _createValidator;    // Stores the instance of the IValidator interface (that AbstractValidator<CreateEmployeeDto> automatically implements) for validating CreateEmployeeDto objects
        private readonly IValidator<UpdateEmployeeDto> _updateValidator;    // The IValidator interface is provided by the FluentValidation library, and UpdateEmployeeValidator automatically satisfies it because it inherits from AbstractValidator<UpdateEmployeeDto>
        private readonly IValidator<BulkCreateEmployeesDto> _bulkCreateValidator;
        private readonly IValidator<BulkDeleteEmployeesDto> _bulkDeleteValidator;
        public EmployeeController(
            EmployeeService employeeService,
            IValidator<CreateEmployeeDto> createValidator,                  // We could have used the CreateEmployeeValidator directly, but using the IValidator interface makes the code more flexible and easier to test
            IValidator<UpdateEmployeeDto> updateValidator,
            IValidator<BulkCreateEmployeesDto> bulkCreateValidator,
            IValidator<BulkDeleteEmployeesDto> bulkDeleteValidator)                  
        {
            _employeeService = employeeService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _bulkCreateValidator = bulkCreateValidator;
            _bulkDeleteValidator = bulkDeleteValidator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var employees = await _employeeService.GetAllAsync();                           // Calls the GetAllAsync method of the EmployeeService and awaits its completion
            return Ok(employees);                                                           // Returns a 200 OK response with the list of employees in the response body
        }

        [HttpGet("{id}")]                                                                   // This attribute will handle GET requests to the /api/employee/{id} endpoint, when someone wants to get a specific employee by their ID. The {id} in the route is a placeholder for the actual employee ID that will be provided in the request URL.
        public async Task<IActionResult> GetById(int id)
        {
            var employee = await _employeeService.GetByIdAsync(id);                         // Calls the GetByIdAsync method of the EmployeeService
            if (employee is null)                                                           // If the employee is not found (it means this variable currently points to no object)
                return NotFound(new { message = $"Employee with ID {id} not found." });     // Remember, EmployeeService.GetByIdAsync returns null if no employee matches. This is where that null gets translated into a proper 404 Not Found HTTP response

            return Ok(employee);                                                            // Else return a 200 OK response with the employee data in the response body
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateEmployeeDto dto)                      // This method takes the CreateEmployeeDto object from the request body [FromBody] automatically because of the [ApiController] attribute by looking at the Action method's parameter type and applies the INFERENCE RULES.
        {
            var validationResult = await _createValidator.ValidateAsync(dto);               // Calls the ValidateAsync method of the CreateEmployeeValidator to validate the incoming DTO. This method checks all the rules defined in the validator and returns a ValidationResult object that contains information about whether the validation passed or failed
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);                                 // If the validation fails, return a 400 Bad Request response with the validation errors in the response body. The validationResult.Errors property contains a list of validation failures, each of which includes information about the property that failed validation and the error message associated with that failure.

            var created = await _employeeService.CreateEmployeeAsync(dto);                  // Calls the CreateEmployeeAsync method of the EmployeeService to create a new employee record in the db.
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);      // CreatedAtAction(...) : the confirmation + directions back to it. It Returns a 201 Created response with the location of the newly created employee resource in the Location header and the created employee data in the response body.
        }                                                                                   // nameof(GetById) is used to generate the URL for the Location header. Since we need to provide the URL of particularly the newly created resource, we use GetById action whose route is /api/employee/{id}. here nameof(...) is a C# feature that returns the name of the method as a string automatically , if we ever rename the GetById method, this code will still work correctly.
                                                                                            // CreatedAtAction (...) bacsically look up GetById in the route table and find its template "api/patients/{id}" then substitutes the {id} placeholder from new { id = created.Id } and returns the full URL with a newly created object body in the response (created).
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateEmployeeDto dto)              // id comes from the URL and dto from the JSON body.
        {
            var validationResult = await _updateValidator.ValidateAsync(dto);               // Calls the ValidateAsync method of the UpdateEmployeeValidator to validate the incoming DTO. This method checks all the rules defined in the validator and returns a ValidationResult object that contains information about whether the validation passed or failed
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);                                 // If the validation fails, return a 400 Bad Request response with the validation errors in the response body.

            var updated = await _employeeService.UpdateEmployeeAsync(id, dto);              // Calls the UpdateEmployeeAsync method of the EmployeeService to update an existing employee record in the db.
            if (updated is null)
                return NotFound(new { message = $"Employee with ID {id} not found." });     // If no such Employee exists, return a 404 Not Found response

            return Ok(updated);                                                             // success, 200 OK with the updated employee as JSON.
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _employeeService.DeleteEmployeeAsync(id);                   // Calls the DeleteEmployeeAsync method of the EmployeeService to delete an existing employee record in the db.
            if (!deleted)
                return NotFound(new { message = $"Employee with ID {id} not found." });     // If no such Employee exists, return a 404 Not Found response

            return NoContent();                                                             // success, 204 No Content. The deletion was successful, and there is no content to return in the response body.
        }

        [HttpPost("bulk")]
        public async Task<IActionResult> BulkCreate(BulkCreateEmployeesDto dto)
        {
            var validationResult = await _bulkCreateValidator.ValidateAsync(dto);           // Calls the ValidateAsync method of the BulkCreateEmployeesValidator to validate the incoming DTO.
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);                                 // If the validation fails, return a 400 Bad Request response with the validation errors in the response body.

            var created = await _employeeService.BulkCreateAsync(dto);                      // Calls the BulkCreateAsync method of the EmployeeService to create multiple employee records in the db.
            return StatusCode(StatusCodes.Status201Created, created);                       // Returns a 201 Created response with the list of created employees in the response body. StatusCode(...) is used here instead of CreatedAtAction(...) because we are creating multiple resources and there is no single resource to point to (the same reasoning as the register endpoint)
        }

        [HttpDelete("bulk")]
        public async Task<IActionResult> BulkDelete([FromBody] BulkDeleteEmployeesDto dto)
        {
            var validationResult = await _bulkDeleteValidator.ValidateAsync(dto);           // Same as above, but for BulkDeleteEmployeesDto
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var result = await _employeeService.BulkDeleteAsync(dto);
            return Ok(result);                                                              // 200 OK with the summary, since we return data (DeletedCount, NotFoundIds) rather than 204 No Content
        }
    }
}