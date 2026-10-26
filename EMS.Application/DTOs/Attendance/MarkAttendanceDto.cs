using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Application.DTOs.Attendance
{
    public class MarkAttendanceDto                          // Direction : IN (we mark the attendance)
    {
        public int EmployeeId { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}