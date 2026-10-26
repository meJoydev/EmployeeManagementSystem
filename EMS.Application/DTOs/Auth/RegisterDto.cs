using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Application.DTOs.Auth         // Direction : IN (What we send when registering a new user)
{
    public class RegisterDto                // a safe, temporary holding container for the raw password, on its way to being hashed.
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}