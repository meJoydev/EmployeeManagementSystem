using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Application.DTOs.Employee                 // Direction : IN (What we write to delete which employee)
{
    public class BulkDeleteEmployeesDto                 // The incoming JSON is { "ids": [4, 5, 6] }
    {
        public List<int> Ids { get; set; } = new();     // The list of employee IDs to be deleted
    }
}