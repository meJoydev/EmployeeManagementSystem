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
    public class SalaryRecordConfiguration : IEntityTypeConfiguration<SalaryRecord>
    {
        public void Configure(EntityTypeBuilder<SalaryRecord> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.BaseSalary).HasColumnType("decimal(10,2)");
            builder.Property(s => s.Deductions).HasColumnType("decimal(10,2)");
            builder.Property(s => s.NetSalary).HasColumnType("decimal(10,2)");

            builder.HasOne(s => s.Employee)             // This SalaryRecord only belongs to one Employee
                   .WithMany()                          // One employee can have many salary records, but we don't need to navigate from Employee to SalaryRecord in this case, so to keep it clean and lightweight we leave it empty (one-way relationship).
                   .HasForeignKey(s => s.EmployeeId)    // EmployeeId is the foreign key in SalaryRecord that references the Employee entity
                   .OnDelete(DeleteBehavior.Cascade);   // history automatically deleted with the employee

            builder.HasIndex(s => new { s.EmployeeId, s.Month, s.Year }).IsUnique();    // index on multiple columns to ensure only one salary record per employee per month
        }
    }
}