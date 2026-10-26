using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Domain.Entities
{
    public class Employee
    {
        public int Id { get; private set; }
        public string FirstName { get; private set; } = null!;
        public string LastName { get; private set; } = null!;
        public string Email { get; private set; } = null!;
        public int DepartmentId { get; private set; }                       // Foreign Key to Department table
        public Department Department { get; private set; } = null!;         // Navigation Property : Employee -> Department | allows us to access the department details of an employee
        public DateTime DateOfJoining { get; private set; }
        public decimal Salary { get; private set; }
        public bool IsActive { get; private set; } = true;                  // For soft deletion, we can mark an employee as inactive instead of deleting the record from the database

        private Employee() { }                                              // For EF Core (EF Core reads the created employee from MySQL) to build a blank object first and then fills in the properties with data(Id=1, FirstName="Rahul",etc...) using reflection which can bypass the private set protections (EF Core Reads the row in Employee table via MYSQL and fills in every property field by field)
                                                                            // This constructor is private to prevent creating an incomplete/invalid employee object(no first name, no email, etc).
        public Employee(string firstName, string lastName, string email, int departmentId, decimal salary)  // This constructor is called when a new employee is created. Then The object is saved to MySQL as a new row.
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            DepartmentId = departmentId;
            Salary = salary;
            DateOfJoining = DateTime.UtcNow;                                // Why it's not in parameter? Because we want to set the date of joining to the current date when the employee is created, and we don't want to allow the user to set it manually.
        }

        public void Deactivate() => IsActive = false;                       // This method allows us to mark an employee as inactive, which is useful for soft deletion scenarios where we want to retain the employee's data in the database but prevent them from being considered active in the system.
        public void UpdateSalary(decimal newSalary) => Salary = newSalary;  // This method allows us to update the salary of an employee
        public void UpdateDetails(string firstName, string lastName, string email, int departmentId)        // This method allows us to update the details of an employee
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            DepartmentId = departmentId;
        }
    }
}
// NOTE : public Employee() is for creating a new employee object and store inside the database, whereas private Employee() is for EF Core for loading (rebuilt employee from the database).