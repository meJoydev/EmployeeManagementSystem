using BCrypt.Net;
using DocumentFormat.OpenXml.Math;
using EMS.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Infrastructure.Security
{
    public class PasswordHasher : IPasswordHasher
    {
        public string Hash(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);            // Use BCrypt's hashing algorithm, get back the scrambled hash string.
        }

        public bool Verify(string password, string passwordHash)        // Verify the provided password against the stored hashed password for the user.
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
    }
}

// NOTE : BCrypt automatically generates something called a salt (a small random value mixed into the password before hashing) and embeds it directly into the resulting hash string.
// Which results in a unique hash for each password.
// Without it, two users with the identical password would produce the exact same hash meaning if an attacker ever recognized one common hash pattern, they'd instantly know every account sharing that password