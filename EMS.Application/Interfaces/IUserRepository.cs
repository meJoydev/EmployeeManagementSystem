using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMS.Domain.Entities;

namespace EMS.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);      // Used during login, find this user using their email so I can check their password. It can return null if no user found by that email
        Task<bool> EmailExistsAsync(string email);      // Used during registration, To check if this email is already signed up or not before registering a new email.
        Task<User> AddAsync(User user);                 // Saves a new user, returns it back with its real database-assigned Id
    }
}