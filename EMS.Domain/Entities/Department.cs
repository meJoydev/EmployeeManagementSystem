using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Domain.Entities
{
    public class Department
    {
        public int Id { get; private set; }
        public string Name { get; private set; } = null!;

        private readonly List<Employee> _employees = new();                             // This is a private list that will hold the employees of this department. We use a private list to encapsulate the collection and prevent external code from modifying it directly, which helps maintain the integrity of the data.
        public IReadOnlyCollection<Employee> Employees => _employees.AsReadOnly();      // Use : Department growth tracking | allows us to access the employees of a department (outsiders can look but can’t .Add() directly)

        private Department() { }                                                        // for EF Core to rebuild objects from the database. It builds a blank object first and then fills in the properties with the data from the database through reflection which can bypass the private set protections.

        public Department(string name)                                                  // For creating a new department object and store inside the database. This constructor is called when a new department is created. Then The object is saved to MySQL as a new row.
        {
            Name = name;
        }
    }
}