using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using EMS.Application.DTOs.Employee;

namespace EMS.Application.Validators
{
    public class BulkDeleteEmployeesValidator : AbstractValidator<BulkDeleteEmployeesDto>
    {
        public BulkDeleteEmployeesValidator()       // in this validator, the list must be non-empty and at most 100, and each ID must be positive (GreaterThan(0))
        {
            RuleFor(x => x.Ids)
                .NotEmpty().WithMessage("At least one employee ID is required.")
                .Must(list => list.Count <= 100).WithMessage("You can delete at most 100 employees at once.");

            RuleForEach(x => x.Ids)
                .GreaterThan(0).WithMessage("Employee IDs must be positive numbers.");
        }
    }
}