using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using EMS.Application.DTOs.Department;

namespace EMS.Application.Validators
{
    public class CreateDepartmentValidator : AbstractValidator<CreateDepartmentDto>                 // For validating the CreateDepartmentDto object
    {
        public CreateDepartmentValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Department name is required.")
                .MaximumLength(50).WithMessage("Department name cannot exceed 50 characters.");    // If error occurs, We catch it here with a friendly message instead of letting MySQL reject it with an ugly error.
        }
    }
}