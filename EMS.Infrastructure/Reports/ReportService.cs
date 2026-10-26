using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMS.Application.Interfaces;
using EMS.Domain.Entities;

namespace EMS.Infrastructure.Reports
{
    public class ReportService : IReportService                 // ReportService implements IReportService interface just by calling the alredy implemented methods in PdfReportGenerator and ExcelReportGenerator classes.
    {                                                           // We could've defined both PdfReportGenerator and ExcelReportGenerator methods here directly without defining seperate class for each report but that would have made this class too long and hard to read. So we separated them following the Single Responsibility Principle.
        private readonly PdfReportGenerator _pdfGenerator;      // _pdfGenerator : stores the instance of PdfReportGenerator class
        private readonly ExcelReportGenerator _excelGenerator;  // _excelGenerator : stores the instance of ExcelReportGenerator class

        public ReportService(PdfReportGenerator pdfGenerator, ExcelReportGenerator excelGenerator)      // Takes the context of PdfReportGenerator and ExcelReportGenerator classes
        {
            _pdfGenerator = pdfGenerator;
            _excelGenerator = excelGenerator;
        }

        public byte[] GenerateEmployeeDirectoryPdf(IEnumerable<Employee> employees)         // => is a shorthand for {...return T}
            => _pdfGenerator.GenerateEmployeeDirectoryPdf(employees);                       // Calls the GenerateEmployeeDirectoryPdf method of PdfReportGenerator class and returns the generated PDF report as byte[] to the API controller which can return it to the client as a downloadable file.

        public byte[] GenerateEmployeeDirectoryExcel(IEnumerable<Employee> employees)
            => _excelGenerator.GenerateEmployeeDirectoryExcel(employees);

        public byte[] GenerateDepartmentReportPdf(IEnumerable<Department> departments)
            => _pdfGenerator.GenerateDepartmentReportPdf(departments);

        public byte[] GenerateDepartmentReportExcel(IEnumerable<Department> departments)
            => _excelGenerator.GenerateDepartmentReportExcel(departments);

        public byte[] GenerateAttendanceReportPdf(IEnumerable<Attendance> attendanceRecords)
            => _pdfGenerator.GenerateAttendanceReportPdf(attendanceRecords);

        public byte[] GenerateAttendanceReportExcel(IEnumerable<Attendance> attendanceRecords)
            => _excelGenerator.GenerateAttendanceReportExcel(attendanceRecords);

        public byte[] GenerateSalaryReportPdf(IEnumerable<SalaryRecord> salaryRecords)
            => _pdfGenerator.GenerateSalaryReportPdf(salaryRecords);

        public byte[] GenerateSalaryReportExcel(IEnumerable<SalaryRecord> salaryRecords)
            => _excelGenerator.GenerateSalaryReportExcel(salaryRecords);
    }
}