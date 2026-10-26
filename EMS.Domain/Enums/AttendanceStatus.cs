using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Domain.Enums
{
    public enum AttendanceStatus        // Used Enums to prevent typos and to restrict the values to a specific set of predefined constants. This ensures that the attendance status can only be one of the defined values, improving code readability and maintainability.
    {                                   // EF Core stores enums as integers by default (0, 1, 2, 3) instead of repeating the text “Present” or “Absent” etc.
        Present,
        Absent,
        Leave,
        HalfDay
    }
}