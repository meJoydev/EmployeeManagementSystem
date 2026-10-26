using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Excel;
using EMS.Domain.Entities;

namespace EMS.Infrastructure.Reports
{
    public class ExcelReportGenerator
    {
        public byte[] GenerateEmployeeDirectoryExcel(IEnumerable<Employee> employees)
        {
            using var workbook = new XLWorkbook();                          // XLWorkbook represents the entire Excel file
            var worksheet = workbook.Worksheets.Add("Employee Directory");  // Worksheets.Add(...) creates one tab/sheet inside it, named "Employee Directory"

            worksheet.Cell(1, 1).Value = "Name";                            // Cell(1, 1) means row 1, column 1 which is A1 in Excel terms (one cell at a time.)
            worksheet.Cell(1, 2).Value = "Email";
            worksheet.Cell(1, 3).Value = "Department";
            worksheet.Cell(1, 4).Value = "Status";
            worksheet.Row(1).Style.Font.Bold = true;                        // Makes the entire first row bold

            int row = 2;                                                    // Since Row 1 is for Headers
            foreach (var employee in employees)                             // traversing employee list for writing each employee's data
            {
                worksheet.Cell(row, 1).Value = $"{employee.FirstName} {employee.LastName}";
                worksheet.Cell(row, 2).Value = employee.Email;
                worksheet.Cell(row, 3).Value = employee.Department?.Name ?? "Unassigned";
                worksheet.Cell(row, 4).Value = employee.IsActive ? "Active" : "Inactive";
                row++;
            }

            worksheet.Columns().AdjustToContents();     // Auto-resizes every column's width to fit whatever text is inside it

            using var stream = new MemoryStream();      // Using statement for automatic cleanup of resources. MemoryStream(a temporary in-memory container) is a way of building a file entirely in memory (to hold raw bytes inside RAM memory), without ever touching your computer's actual disk/hard drive
            workbook.SaveAs(stream);                    // Take everything i built into this workbook object, and write it into that temporary bucket as real Excel file bytes.
            return stream.ToArray();                    // stream.ToArray() converts it into the raw byte[]
        }

        public byte[] GenerateDepartmentReportExcel(IEnumerable<Department> departments)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Departments");

            worksheet.Cell(1, 1).Value = "Department Name";
            worksheet.Cell(1, 2).Value = "Employee Count";
            worksheet.Row(1).Style.Font.Bold = true;

            int row = 2;
            foreach (var department in departments)
            {
                worksheet.Cell(row, 1).Value = department.Name;
                worksheet.Cell(row, 2).Value = department.Employees.Count;
                row++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public byte[] GenerateAttendanceReportExcel(IEnumerable<Attendance> attendanceRecords)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Attendance");

            worksheet.Cell(1, 1).Value = "Employee Name";
            worksheet.Cell(1, 2).Value = "Date";
            worksheet.Cell(1, 3).Value = "Status";
            worksheet.Row(1).Style.Font.Bold = true;

            int row = 2;
            foreach (var record in attendanceRecords)
            {
                worksheet.Cell(row, 1).Value = $"{record.Employee.FirstName} {record.Employee.LastName}";
                worksheet.Cell(row, 2).Value = record.Date.ToString("dd-MMM-yyyy");
                worksheet.Cell(row, 3).Value = record.Status.ToString();
                row++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public byte[] GenerateSalaryReportExcel(IEnumerable<SalaryRecord> salaryRecords)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Salary");

            worksheet.Cell(1, 1).Value = "Employee Name";
            worksheet.Cell(1, 2).Value = "Month/Year";
            worksheet.Cell(1, 3).Value = "Base Salary";
            worksheet.Cell(1, 4).Value = "Deductions";
            worksheet.Cell(1, 5).Value = "Net Salary";
            worksheet.Row(1).Style.Font.Bold = true;

            int row = 2;
            foreach (var record in salaryRecords)
            {
                worksheet.Cell(row, 1).Value = $"{record.Employee.FirstName} {record.Employee.LastName}";
                worksheet.Cell(row, 2).Value = $"{record.Month}/{record.Year}";
                worksheet.Cell(row, 3).Value = record.BaseSalary;
                worksheet.Cell(row, 4).Value = record.Deductions;
                worksheet.Cell(row, 5).Value = record.NetSalary;
                row++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}