using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EMS.Domain.Entities;

namespace EMS.Infrastructure.Persistence.Configurations
{
    public class AttendanceConfiguration : IEntityTypeConfiguration<Attendance>
    {
        public void Configure(EntityTypeBuilder<Attendance> builder)
        {
            builder.HasKey(a => a.Id);

            builder.HasOne(a => a.Employee)                 // This Attendance record only belongs to one Employee
                   .WithMany()                              // One employee can have many attendance records, but we don't need to navigate from Employee to Attendance in this case, so to keep it clean and lightweight we leave it empty (one-way relationship).
                   .HasForeignKey(a => a.EmployeeId)
                   .OnDelete(DeleteBehavior.Cascade);       // If an employee record is ever deleted, their old attendance history will also be deleted automatically.

            builder.HasIndex(a => new { a.EmployeeId, a.Date }).IsUnique();     // [Indexing] it creates a combined unique index across both columns together to find a unique record to prevent duplicate attendance records for the same employee. 
        }
    }
}

// .WithMany() : We can look at an Attendance record and immediately see which Employee it belongs to (attendance.Employee). However, we cannot grab an Employee object and type employee.Attendances because that property does not exist on the class.
