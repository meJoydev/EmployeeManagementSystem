using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using EMS.Application.DTOs.Employee;

namespace EMS.Application.Validators
{
    public class BulkCreateEmployeesValidator : AbstractValidator<BulkCreateEmployeesDto>       // gives us RuleFor/RuleForEach and makes the class count as IValidator<...>
    {
        public BulkCreateEmployeesValidator()
        {
            RuleFor(x => x.Employees)
                .NotEmpty().WithMessage("At least one employee is required.")
                .Must(list => list.Count <= 100).WithMessage("You can create at most 100 employees at once.");

            RuleForEach(x => x.Employees)                                                       // RuleForEach is used for validating each item in a collection, not the list as a whole
                .NotNull().WithMessage("Employee entries cannot be null.")
                .SetValidator(new CreateEmployeeValidator());                                   // SetValidator allows us to use another validator for each item in the collection which means reusing the single-employee rules (name required, valid email, salary > 0, department selected) for every item. 
        }                                                                                       // You write those rules once. Errors come back with the position, like Employees[2].Email, so you know which row to fix.
    }
}