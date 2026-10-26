using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using EMS.Domain.Entities;

namespace EMS.Infrastructure.Reports
{
    public class PdfReportGenerator
    {
        public PdfReportGenerator()                                     // A constructor that runs each time ASP.NET Core creates this class, which is before any PDF is generated.
        {
            QuestPDF.Settings.License = LicenseType.Community;          // Tells QuestPDF “I’m using the free Community license.” Without this line, it throws an exception when it first builds a document.
        }
        public byte[] GenerateEmployeeDirectoryPdf(IEnumerable<Employee> employees)     // Implementing the Employee Directory for pdf generation
        {
            var document = Document.Create(container =>                 // [STEP 1] : Creating and describing what the document should look like (similar to HTML) and QuestPDF hands a container to work with
            {
                container.Page(page =>                                  // Describes this document has one page and QuestPDF hands a page
                {
                    page.Margin(30);                                    // [STEP 2] : Leaves 30 points of spaces from all four sides

                    page.Header()                                       // [STEP 3] : Defines what appears at the top of every page
                        .Text("Employee Directory")                     // Note : If it spans multiple pages then this title repeats on each page automatically
                        .FontSize(20).Bold();

                    page.Content().Table(table =>                       // [STEP 4] : page.Content() defines the main body, .Table(table =>...) for building the tables
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();                   // RelativeColumn() means "split the available width evenly"
                            columns.RelativeColumn();                   // four of them means each column gets roughly 25% of the page width
                            columns.RelativeColumn();                   // We can wegiht them differently later, eg: RelativeColumn(2) for a wider "Name" column
                            columns.RelativeColumn();
                        });

                        table.Header(header =>                          // [STEP 5] : Defines the column titles row i,e : Name, Email, Department, Status in Bold
                        {
                            header.Cell().Text("Name").Bold();
                            header.Cell().Text("Email").Bold();
                            header.Cell().Text("Department").Bold();
                            header.Cell().Text("Status").Bold();
                        });

                        foreach (var employee in employees)                                     // looping the employee list 
                        {
                            table.Cell().Text($"{employee.FirstName} {employee.LastName}");     // table.Cell() : Creates one cell in the table, .Text : Add text into that cell
                            table.Cell().Text(employee.Email);
                            table.Cell().Text(employee.Department?.Name ?? "Unassigned");       // employee.Department?.Name : where ? means if the Department is missing/null then give me null instead of crashing orelse Show this employee's department name.  ?? "Unassigned" means if its null, Show Unassigned.
                            table.Cell().Text(employee.IsActive ? "Active" : "Inactive");
                        }
                    });

                    page.Footer()                                                   // Small details that appears at the bottom of every page
                        .AlignCenter()
                        .Text(text =>                                    
                        {
                            text.Span("Generated on ");                             // text.Span() : standalone fragment of plain text                         
                            text.Span(DateTime.Now.ToString("dd-MMM-yyyy"));        // .Span() allows to chain specific styling rules (like bolding, colors, or font sizes) to just that piece of text leaving the rest of the sentence alone
                        });
                });
            });

            return document.GeneratePdf();              // Renders the pdf file and returns as byte[]
        }

        public byte[] GenerateDepartmentReportPdf(IEnumerable<Department> departments)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);

                    page.Header()
                        .Text("Department Report")
                        .FontSize(20).Bold();

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Department Name").Bold();
                            header.Cell().Text("Employee Count").Bold();
                        });

                        foreach (var department in departments)
                        {
                            table.Cell().Text(department.Name);
                            table.Cell().Text(department.Employees.Count.ToString());
                        }
                    });

                    page.Footer()
                        .AlignCenter()
                        .Text(text =>
                        {
                            text.Span("Generated on ");
                            text.Span(DateTime.Now.ToString("dd-MMM-yyyy"));
                        });
                });
            });

            return document.GeneratePdf();
        }

        public byte[] GenerateAttendanceReportPdf(IEnumerable<Attendance> attendanceRecords)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);

                    page.Header()
                        .Text("Attendance Report")
                        .FontSize(20).Bold();

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Employee Name").Bold();
                            header.Cell().Text("Date").Bold();
                            header.Cell().Text("Status").Bold();
                        });

                        foreach (var record in attendanceRecords)
                        {
                            table.Cell().Text($"{record.Employee.FirstName} {record.Employee.LastName}");
                            table.Cell().Text(record.Date.ToString("dd-MMM-yyyy"));
                            table.Cell().Text(record.Status.ToString());
                        }
                    });

                    page.Footer()
                        .AlignCenter()
                        .Text(text =>
                        {
                            text.Span("Generated on ");
                            text.Span(DateTime.Now.ToString("dd-MMM-yyyy"));
                        });
                });
            });

            return document.GeneratePdf();
        }

        public byte[] GenerateSalaryReportPdf(IEnumerable<SalaryRecord> salaryRecords)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);

                    page.Header()
                        .Text("Salary Report")
                        .FontSize(20).Bold();

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Employee Name").Bold();
                            header.Cell().Text("Month/Year").Bold();
                            header.Cell().Text("Base Salary").Bold();
                            header.Cell().Text("Deductions").Bold();
                            header.Cell().Text("Net Salary").Bold();
                        });

                        foreach (var record in salaryRecords)
                        {
                            table.Cell().Text($"{record.Employee.FirstName} {record.Employee.LastName}");
                            table.Cell().Text($"{record.Month}/{record.Year}");
                            table.Cell().Text(record.BaseSalary.ToString("N2"));
                            table.Cell().Text(record.Deductions.ToString("N2"));
                            table.Cell().Text(record.NetSalary.ToString("N2"));
                        }
                    });

                    page.Footer()
                        .AlignCenter()
                        .Text(text =>
                        {
                            text.Span("Generated on ");
                            text.Span(DateTime.Now.ToString("dd-MMM-yyyy"));
                        });
                });
            });

            return document.GeneratePdf();
        }
    }
}