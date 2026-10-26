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
    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>     // This class specifically configures how the Employee entity maps to its database table
    {                                                                           // Implementing IEntityTypeConfiguration<Employee> is what lets ApplyConfigurationsFromAssembly (in DbContext) find and use this file automatically.
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.HasKey(e => e.Id);                                          // Set the id column as the primary key for the Employee entity

            builder.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
            builder.Property(e => e.LastName).IsRequired().HasMaxLength(50);
            builder.Property(e => e.Email).IsRequired().HasMaxLength(100);
            builder.HasIndex(e => e.Email).IsUnique();

            builder.Property(e => e.Salary).HasColumnType("decimal(10,2)");     // Set Salary number up to 10 total digits, with 2 of them after the decimal point

            builder.HasOne(e => e.Department)               // An Employee has one Department
                   .WithMany(d => d.Employees)              // A Department has many Employees
                   .HasForeignKey(e => e.DepartmentId)      // Employee has a foreign key DepartmentId that references the Department entity
                   .OnDelete(DeleteBehavior.Restrict);      // If a department is deleted, the employees in that department will not be deleted. It throws a database exception.
        }
    }
}