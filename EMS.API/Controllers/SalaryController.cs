using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EMS.Application.DTOs.Salary;
using EMS.Application.Services;

namespace EMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]                                                                     // Only logged-in users touch salary data.
    public class SalaryController : ControllerBase
    {
        private readonly SalaryService _salaryService;

        public SalaryController(SalaryService salaryService)                        // Dependency injection of the SalaryService to handle salary-related operations.
        {
            _salaryService = salaryService;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> Generate(GenerateSalaryDto dto)            // Endpoint to generate monthly salary records based on the provided month and year.
        {
            //try                                                                   // NOTE : The try-catch block is commented out because the global exception handler will catch the ArgumentException thrown by the service and return a 400 Bad Request response.
            //{
            var count = await _salaryService.GenerateMonthlySalaryAsync(dto.Month, dto.Year);   // Calls the service to generate salary records and returns the count of records generated.
            return Ok(new { message = $"{count} salary record(s) generated for {dto.Month}/{dto.Year}." });
            //}
            //catch (ArgumentException ex)                                          // Catches the invalid-month or invalid-year errors we throw in the service and turns them into 400 Bad Request.
            //{
            //    return BadRequest(new { message = ex.Message });
            //}
        }

        [HttpGet("month/{month}/year/{year}")]                                      // GET api/salary/month/8/year/2026, which returns everyone’s salary for that month
        public async Task<IActionResult> GetByMonthYear(int month, int year)        // Endpoint to retrieve salary records for a specific month and year.
        {
            var records = await _salaryService.GetByMonthYearAsync(month, year);    // Calls the service to get salary records
            return Ok(records);                                                     // Returns the records in the response body with a 200 OK status.
        }

        [HttpGet("employee/{employeeId}")]
        public async Task<IActionResult> GetByEmployee(int employeeId)              // Endpoint to retrieve salary records/history for a specific employee.
        {
            var records = await _salaryService.GetByEmployeeAsync(employeeId);
            return Ok(records);
        }
    }
}