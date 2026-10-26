using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using EMS.Application.DTOs.Employee;

namespace EMS.Application.Validators
{
    public class CreateEmployeeValidator : AbstractValidator<CreateEmployeeDto>     // AbstractValidator : A base class provided by FluentValidation 
    {                                                                               // AbstractValidator provides a set of methods and properties that allow you to define validation rules for the properties of the CreateEmployeeDto class like RuleFor, NotEmpty, EmailAddress, GreaterThan, WithMessage etc.
        public CreateEmployeeValidator()                                            // AbstractValidator also implements the IValidator<T> interface automatically
        {
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("First name is required.");
            RuleFor(x => x.LastName).NotEmpty().WithMessage("Last name is required.");
            RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("A valid email is required.");
            RuleFor(x => x.Salary).GreaterThan(0).WithMessage("Salary must be greater than zero.");
            RuleFor(x => x.DepartmentId).GreaterThan(0).WithMessage("A valid department must be selected.");
        }
    }
}