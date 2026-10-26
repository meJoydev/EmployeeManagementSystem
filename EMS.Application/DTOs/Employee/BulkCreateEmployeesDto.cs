using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Application.DTOs.Employee                                     // Direction : IN
{
    public class BulkCreateEmployeesDto                                     // the incoming JSON is { "employees": [ {...}, {...} ] }. It reuses CreateEmployeeDto for each item, so the single-create and bulk-create shapes can’t drift apart.
    {
        public List<CreateEmployeeDto> Employees { get; set; } = new();     // List of CreateEmployeeDto objects representing the employees to be created where new() means the list starts empty instead of null, the same “never null” reasoning as = string.Empty elsewhere.
    }
}