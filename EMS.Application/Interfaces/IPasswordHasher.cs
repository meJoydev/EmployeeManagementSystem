using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Application.Interfaces
{
    public interface IPasswordHasher
    {
        string Hash(string password);                       // (For New Users) Takes a real, plain-text password, and returns the scrambled/hashed version — used once, during registration.
        bool Verify(string password, string passwordHash);  // (For Existing Users) Takes the plain-text password, plus the stored hash from the database and compares them and returns true/false ("shred and compare" logic).
    }
}