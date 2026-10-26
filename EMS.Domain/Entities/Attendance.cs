using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EMS.Domain.Enums;

namespace EMS.Domain.Entities
{
    public class Attendance
    {
        public int Id { get; private set; }
        public int EmployeeId { get; private set; }                 // Foreign Key to Employee table
        public Employee Employee { get; private set; } = null!;     // Navigation Property : Attendance → Employee | allows us to access the employee details of an attendance record
        public DateTime Date { get; private set; }
        public AttendanceStatus Status { get; private set; }

        private Attendance() { }                                    // for EF Core to rebuild objects from the database.
        public Attendance(int employeeId, DateTime date, AttendanceStatus status)   // for creating a new attendance record object and store inside the database.
        {
            EmployeeId = employeeId;
            Date = date.Date;
            Status = status;
        }

        public void UpdateStatus(AttendanceStatus newStatus) => Status = newStatus;
    }
}