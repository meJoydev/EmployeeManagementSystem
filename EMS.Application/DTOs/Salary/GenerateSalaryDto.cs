using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Application.DTOs.Salary
{
    public class GenerateSalaryDto              // Direction : IN (What we send when generating salary for a specific month and year)
    {
        public int Month { get; set; }
        public int Year { get; set; }
    }
}