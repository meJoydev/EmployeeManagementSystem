using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EMS.Application.DTOs.Attendance;
using EMS.Application.Services;

namespace EMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]                                                                     // Every single action here requires a valid, logged-in JWT token.
    public class AttendanceController : ControllerBase
    {
        private readonly AttendanceService _attendanceService;

        public AttendanceController(AttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }

        [HttpPost("mark")]                                                          // Post because we are creating a new attendance record or updating an existing one. Not Put/Patch because it's only meant for pure resource updates and not for creating new resources.
        public async Task<IActionResult> MarkAttendance(MarkAttendanceDto dto)      // [FromBody] is inferred automatically because of the [ApiController] attribute.
        {
            //try                                                                   // NOTE : The try-catch block is commented out because the global exception handler will catch the ArgumentException thrown by the service and return a 400 Bad Request response.
            //{
            await _attendanceService.MarkAttendanceAsync(dto);                      // MarkAttendanceAsync does the string-to-enum conversion, checks for an existing record for that employee+date, and either updates or creates accordingly.
            return Ok(new { message = "Attendance marked successfully." });         // Return a 200 OK response with a success message in the response body
            //}
            //catch (ArgumentException ex)                                          // If the string-to-enum conversion fails, MarkAttendanceAsync throws an ArgumentException as mentioned inside the service, which is caught here. The controller then returns a 400 Bad Request response with the error message to the client.
            //{
            //    return BadRequest(new { message = ex.Message });
            //}
        }

        [HttpGet("employee/{employeeId}")]                                          // "employee/{employeeId}" combined with the class route produces GET api/attendance/employee/5, fetching employee 5's full attendance history
        public async Task<IActionResult> GetByEmployee(int employeeId)              // To fetch all attendance records for a specific employee by their ID.
        {
            var records = await _attendanceService.GetByEmployeeAsync(employeeId);  // Wait till it Fetches all attendance records for the specified employee from the service, which in turn fetches from the repository and maps to AttendanceResponseDto 
            return Ok(records);
        }

        [HttpGet("date/{date}")]                                                    // "date/{date}" produces GET api/attendance/date/2026-10-07, fetching everyone's attendance for that one day
        public async Task<IActionResult> GetByDate(DateTime date)                   // The DateTime parameter is automatically parsed from the URL segment, and the default format is yyyy-MM-dd. If the date is not in this format, it will return a 400 Bad Request response.
        {
            var records = await _attendanceService.GetByDateAsync(date);
            return Ok(records);
        }                                                                           // Why separate employee/ and date/ prefixes ? Because ASP.NET Core's routing can't tell the difference between api/attendance/5 (meant as an employee ID) and api/attendance/2026-10-07 (meant as a date) just by looking at the raw URL shape.
                                                                                    // both are just "some value" to the router. Adding distinct path segments (employee/, date/) removes that ambiguity completely and tells the router exactly which action a given URL is meant for.
    }
}
