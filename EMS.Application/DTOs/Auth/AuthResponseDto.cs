using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Application.DTOs.Auth                             // Direction : OUT 
{
    public class AuthResponseDto                                // After a successful login/register when user gets saved and a token gets generated, When we need to send it back to React we use AuthResponseDto
    {                                                           // AuthResponseDto acts as a filter to show only Token, UserName & Role and Not the passwords or hashes (Like Abstraction)
        public string Token { get; set; } = string.Empty;       // A signed piece of text that proves "this user is logged in" without needing to authorise the user again and again on every future request.
        public string Username { get; set; } = string.Empty;    // Username/Role let React immediately display "Welcome, [name]"
        public string Role { get; set; } = string.Empty;
    }
}

// NOTE : Why not just send the User entity back rather than AuthResponseDto ? Because PasswordHash will get exposed to API ressponse even though its hashed there's no reason to expose any sensitive data.