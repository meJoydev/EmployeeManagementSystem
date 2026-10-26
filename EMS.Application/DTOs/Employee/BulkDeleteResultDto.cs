using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Application.DTOs.Employee                         // Direction : OUT (What the API returns as response)
{
    public class BulkDeleteResultDto                            // It says how many were deleted and which IDs didn’t exist, so a partially wrong list isn’t silently ignored. 
    {
        public int DeletedCount { get; set; }                   // the same idea as returning createdCount from salary generation.
        public List<int> NotFoundIds { get; set; } = new();
    }
}