using EMS.Application.DTOs.Auth;
using EMS.Application.Exceptions;
using EMS.Application.Interfaces;
using EMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthService(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IJwtTokenGenerator jwtTokenGenerator)                                   // Dependency Injection
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)           // Providing RegisterDto which contains the New user data required to perform registeration of the user.
        {
            var emailExists = await _userRepository.EmailExistsAsync(dto.Email);    // dto.Email is the email address provided by the user during registration.
            if (emailExists)
                throw new ConflictException("An account with this email already exists.");  // If the email already exists in the repository, throw a ConflictException with a message.
                                                                                            //  ConflictException(...) over InvalidOperationException because InvalidOperationException is a generic exception, and we want to provide a more specific exception type to indicate that the operation failed due to a conflict (email already exists).
            var passwordHash = _passwordHasher.Hash(dto.Password);                  // Hash the password provided by the user during registration.
            var user = new User(dto.Username, dto.Email, passwordHash);             // Create a new User object with the provided username, email, and hashed password.
            var createdUser = await _userRepository.AddAsync(user);                 // Add the new user to the repository (database) and get the created user object.

            var token = _jwtTokenGenerator.GenerateToken(createdUser);              // Generate a JWT token for the newly created user.

            return new AuthResponseDto                                              // Return an AuthResponseDto containing the generated token, username, and role of the created user.
            {
                Token = token,
                Username = createdUser.Username,
                Role = createdUser.Role
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);                    // Retrieve the user from the repository (database) based on the provided email address.
            if (user is null)
                throw new UnauthorizedAccessException("Invalid email or password.");        // If the user is not found, show invalid email or password (Generic message for wrong email or password to prevent cyberattacks)

            var passwordValid = _passwordHasher.Verify(dto.Password, user.PasswordHash);    // Verify the provided password against the stored hashed password for the user.
            if (!passwordValid)
                throw new UnauthorizedAccessException("Invalid email or password.");

            var token = _jwtTokenGenerator.GenerateToken(user);                             // Generate a JWT token for the authenticated user.

            return new AuthResponseDto                                                      // Returns an AuthResponseDto (A Filtered user object returned by the AuthService to React/Client to remember the authenticated user)
            {
                Token = token,
                Username = user.Username,
                Role = user.Role
            };
        }
    }
}