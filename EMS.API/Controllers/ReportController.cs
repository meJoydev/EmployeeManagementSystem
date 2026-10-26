using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EMS.Application.Interfaces;

namespace EMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReportController : ControllerBase
    {
        private const string ExcelContentType =                                                     // stores the long, fixed Excel MIME type once under a readable name, so the four Excel download endpoints can reuse it without retyping it
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";                    // This is the MIME type for Excel files in the OpenXML format (used by .xlsx files) that tells the browser “this response is an Excel file.”
                                                                                                    // Need ? The browser uses the MIME type to decide how to treat the download. If the MIME type is missing or incorrect, the browser may not recognize the file as an Excel file and may not open it correctly.
        private readonly IReportService _reportService;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly ISalaryRecordRepository _salaryRecordRepository;

        public ReportController(
            IReportService reportService,                                                           // IReportService makes the files, and the other four repositories fetches the data each report needs.
            IEmployeeRepository employeeRepository,
            IDepartmentRepository departmentRepository,
            IAttendanceRepository attendanceRepository,
            ISalaryRecordRepository salaryRecordRepository)                                         // constructor injection of the service and repositories needed for generating reports
        {
            _reportService = reportService;
            _employeeRepository = employeeRepository;
            _departmentRepository = departmentRepository;
            _attendanceRepository = attendanceRepository;
            _salaryRecordRepository = salaryRecordRepository;
        }

        // ---------- Employee directory ----------

        [HttpGet("employees/pdf")]
        public async Task<IActionResult> GetEmployeeDirectoryPdf()                                  // Endpoint for generating a PDF report of all employees
        {
            var employees = await _employeeRepository.GetAllAsync();                                // Fetch all employees from the repository and returns a list of Employee objects.
            var bytes = _reportService.GenerateEmployeeDirectoryPdf(employees);                     // Generate the PDF report using the report service, controller passes that list to _reportService and Behind the scenes, ReportService forwards the call to PdfReportGenerator, which uses QuestPDF to build the table and returns the PDF as byte[]
            return File(bytes, "application/pdf", "EmployeeDirectory.pdf");                         // Controller wraps those bytes in an HTTP response with the right MIME type and filename, and the browser downloads it
        }

        [HttpGet("employees/excel")]
        public async Task<IActionResult> GetEmployeeDirectoryExcel()                                // Endpoint for generating an Excel report of all employees
        {
            var employees = await _employeeRepository.GetAllAsync();                                // In this step, the other 4 controllers fetches the data each report needs from the repository and returns a list of Employee objects.
            var bytes = _reportService.GenerateEmployeeDirectoryExcel(employees);                   // In this step, reportService generates the reports 
            return File(bytes, ExcelContentType, "EmployeeDirectory.xlsx");                         // Here we'll use the ExcelContentType constant defined at the top of the controller
        }

        // ---------- Departments ----------

        [HttpGet("departments/pdf")]
        public async Task<IActionResult> GetDepartmentReportPdf()                                   // Same pattern as the employee directory endpoints, but for departments instead of employees
        {
            var departments = await _departmentRepository.GetAllAsync();
            var bytes = _reportService.GenerateDepartmentReportPdf(departments);                    // Notice every repository is awaited but here _reportService is not awaited because DB calls 
            return File(bytes, "application/pdf", "DepartmentReport.pdf");
        }

        [HttpGet("departments/excel")]
        public async Task<IActionResult> GetDepartmentReportExcel()                                 // Same as above but in excel
        {
            var departments = await _departmentRepository.GetAllAsync();
            var bytes = _reportService.GenerateDepartmentReportExcel(departments);
            return File(bytes, ExcelContentType, "DepartmentReport.xlsx");
        }

        // ---------- Attendance (for one date; defaults to today) ----------

        [HttpGet("attendance/pdf")]
        public async Task<IActionResult> GetAttendanceReportPdf([FromQuery] DateTime? date)         // The [FromQuery] attribute tells ASP.NET Core to look for the date parameter in the query string of the URL. If the client does not provide a date, the date parameter will be null, and we will use DateTime.UtcNow.Date as the default value.
        {
            var reportDate = date ?? DateTime.UtcNow.Date;                                          // If the date query parameter is not provided, use the current UTC date (without time) as the default report date
            var records = await _attendanceRepository.GetByDateAsync(reportDate);
            var bytes = _reportService.GenerateAttendanceReportPdf(records);
            return File(bytes, "application/pdf", $"AttendanceReport_{reportDate:yyyy-MM-dd}.pdf"); // The resulting string will be used as the filename for the downloaded PDF report.
        }

        [HttpGet("attendance/excel")]
        public async Task<IActionResult> GetAttendanceReportExcel([FromQuery] DateTime? date)
        {
            var reportDate = date ?? DateTime.UtcNow.Date;
            var records = await _attendanceRepository.GetByDateAsync(reportDate);
            var bytes = _reportService.GenerateAttendanceReportExcel(records);
            return File(bytes, ExcelContentType, $"AttendanceReport_{reportDate:yyyy-MM-dd}.xlsx");
        }

        // ---------- Salary (for one month/year) ----------

        [HttpGet("salary/pdf")]
        public async Task<IActionResult> GetSalaryReportPdf([FromQuery] int month, [FromQuery] int year)    // The [FromQuery] attribute tells ASP.NET Core to look for the month and year parameters in the query string of the URL. If the client does not provide these parameters, the method will return a BadRequest response with an error message.
        {
            if (month < 1 || month > 12)
                return BadRequest(new { message = "Month must be between 1 and 12." });

            var records = await _salaryRecordRepository.GetByMonthYearAsync(month, year);
            var bytes = _reportService.GenerateSalaryReportPdf(records);
            return File(bytes, "application/pdf", $"SalaryReport_{month}_{year}.pdf");
        }

        [HttpGet("salary/excel")]
        public async Task<IActionResult> GetSalaryReportExcel([FromQuery] int month, [FromQuery] int year)
        {
            if (month < 1 || month > 12)
                return BadRequest(new { message = "Month must be between 1 and 12." });

            var records = await _salaryRecordRepository.GetByMonthYearAsync(month, year);
            var bytes = _reportService.GenerateSalaryReportExcel(records);
            return File(bytes, ExcelContentType, $"SalaryReport_{month}_{year}.xlsx");
        }
    }
}