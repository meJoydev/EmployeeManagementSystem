using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMS.Domain.Entities;

namespace EMS.Application.Interfaces
{
    public interface IReportService
    {
        byte[] GenerateEmployeeDirectoryPdf(IEnumerable<Employee> employees);           // byte[] holds the binary data(0/1) of the generated PDF report in memory which the API controller can return to the client as a downloadable file.
        byte[] GenerateEmployeeDirectoryExcel(IEnumerable<Employee> employees);
        byte[] GenerateDepartmentReportPdf(IEnumerable<Department> departments);
        byte[] GenerateDepartmentReportExcel(IEnumerable<Department> departments);
        byte[] GenerateAttendanceReportPdf(IEnumerable<Attendance> attendanceRecords);
        byte[] GenerateAttendanceReportExcel(IEnumerable<Attendance> attendanceRecords);
        byte[] GenerateSalaryReportPdf(IEnumerable<SalaryRecord> salaryRecords);
        byte[] GenerateSalaryReportExcel(IEnumerable<SalaryRecord> salaryRecords);
    }
}