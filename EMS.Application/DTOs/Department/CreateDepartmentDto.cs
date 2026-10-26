using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Application.DTOs.Department
{
    public class CreateDepartmentDto                        // Direction : IN (When we create a new department)
    {
        public string Name { get; set; } = string.Empty;    // = string.Empty keeps it from ever being null, the same reasoning as every other DTO (input should not be null)
    }
}