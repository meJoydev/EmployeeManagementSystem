using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Application.DTOs.Attendance
{
    public class AttendanceResponseDto                                  // Direction : OUT (What we get as response if we request attendance information)
    {
        public int Id { get; set; }
        public string EmployeeName { get; set; } = string.Empty;        // since React's attendance table wants to display a name, not an ID.
        public DateTime Date { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}