using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMS.Domain.Entities;

namespace EMS.Application.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(User user);        // Generates a signed token string representing their logged-in session for each user
    }
}