using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EMS.Domain.Entities;

namespace EMS.Infrastructure.Persistence
{
    public class EmsDbContext : DbContext
    {
        public EmsDbContext(DbContextOptions<EmsDbContext> options) : base(options) { }

        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<Department> Departments => Set<Department>();
        public DbSet<Attendance> Attendances => Set<Attendance>();
        public DbSet<SalaryRecord> SalaryRecords => Set<SalaryRecord>();
        public DbSet<User> Users => Set<User>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)      // Find and apply all the entity configurations written in this assembly automatically
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(EmsDbContext).Assembly);    // This line scans your entire project looking for any class that implements IEntityTypeConfiguration<T>, and automatically applies it.
        }
    }
}