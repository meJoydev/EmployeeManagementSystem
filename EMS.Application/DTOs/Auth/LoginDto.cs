using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Application.DTOs.Auth     // Direction : IN (What we send when logging in an existing user)
{
    public class LoginDto               // A container for existing user that stores the raw information provided by the user before user credentials being hashed
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}