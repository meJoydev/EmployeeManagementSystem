using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EMS.Application.Interfaces;
using EMS.Domain.Entities;
using EMS.Infrastructure.Persistence;

namespace EMS.Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly EmsDbContext _context;

        public UserRepository(EmsDbContext context)                 // Taking context from DbContext
        {
            _context = context;
        }

        public async Task<User?> GetByEmailAsync(string email)      // Retrive the user from the DB using the provided email address during login
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);        // FirstOrDefaultAsync returns the first user that matches the email, or null if no user is found
        }

        public async Task<bool> EmailExistsAsync(string email)      // Check if the provided email address already exists in the DB during registration
        {
            return await _context.Users
                .AnyAsync(u => u.Email == email);                   // .AnyAsync(...) is different from .FirstOrDefaultAsync(...) instead of fetching the entire matching row
        }                                                           // it just asks the database "does at least one row match this condition — yes or no? (faster/cheaper)

        public async Task<User> AddAsync(User user)                 // Add a new user to the DB during registration
        {
            await _context.Users.AddAsync(user);                    // Add the new user to the Users DbSet
            await _context.SaveChangesAsync();                      // Save the changes to the database (commit the transaction)
            return user;                                            // Return the newly created user object (with the SQL generated Id)
        }
    }
}