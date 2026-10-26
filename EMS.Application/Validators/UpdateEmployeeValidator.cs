using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using EMS.Application.DTOs.Employee;

namespace EMS.Application.Validators
{
    public class UpdateEmployeeValidator : AbstractValidator<UpdateEmployeeDto>     // a validator specifically for UpdateEmployeeDto, Inheriting AbstractValidator<T> gives us RuleFor, and it implements IValidator<T> interface automatically
    {                                                                               // We don’t need to register it in Program.cs. AddValidatorsFromAssemblyContaining already scans the whole project and picks it up.
        public UpdateEmployeeValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("First name is required.");
            RuleFor(x => x.LastName).NotEmpty().WithMessage("Last name is required.");
            RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("A valid email is required.");
            RuleFor(x => x.Salary).GreaterThan(0).WithMessage("Salary must be greater than zero.");
            RuleFor(x => x.DepartmentId).GreaterThan(0).WithMessage("A valid department must be selected.");
        }
    }
}
