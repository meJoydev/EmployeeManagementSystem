using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Domain.Entities
{
    public class User                                       // Building for Authentication (Login)
    {
        public int Id { get; private set; }                 // Unique id required to log in each user 
        public string Username { get; private set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }    // Stores the password as a scrambled string which cannot be reversed back into the original password
        public string Role { get; private set; }            // Role based login to restrict certain actions to specific roles 

        private User()                                      // Called When Reading an existing user back from DB, Only for EF Core to build a blank object first to import raw data from DB into C# object to work with.
        {                                                   // EF Core Reads the row in Users table via MYSQL and fills in every property field by field using a special mechanism (reflection) that can bypass the private set protections
            Username = null!;                               // Private Constructor to prevent creating incomplete/invalid user object
            Email = null!;
            PasswordHash = null!;
            Role = null!;
        }

        public User(string username, string email, string passwordHash, string role = "HR")     // Called when a New User is get registered
        {                                                   // here Constructor is expecting the password is hashed before the PasswordHash entity is created
            Username = username;
            Email = email;
            PasswordHash = passwordHash;
            Role = role;
        }
    }
}