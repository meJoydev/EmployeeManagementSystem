using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using EMS.Application.Interfaces;
using EMS.Domain.Entities;

namespace EMS.Infrastructure.Security
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly IConfiguration _configuration;                     // IConfiguration is a built-in ASP.NET Core interface that gives you access to your appsettings.json file's values (like a settings reader)

        public JwtTokenGenerator(IConfiguration configuration)              // Constructor injection
        {
            _configuration = configuration;
        }

        public string GenerateToken(User user)                              // STEP 1 : When the user succesfully signed in (email and password both verified correct)
        {                                                                   // STEP 2 : Gather Claims (NOT hidden or encrypted)
            var claims = new List<Claim>                                    // A JWT token is like an ID badge, a Claim is one piece of information printed on that badge.
            {                                                               // Once the token is generated, and handed to React/client, every future request React makes can show this badge instead of logging in again
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),   
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };                                                              
                                                                            // STEP 3 : Load the Secret Key 
            var key = new SymmetricSecurityKey(                             // SymmetricSecurityKey means: this same exact key is used both to create the signature AND to check the signature later
                Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]!));     // _configuration["Jwt:SecretKey"] — reads the secret key from appsettings.json known only to your server. 
                                                                            // Encoding.UTF8.GetBytes(...) just converts that secret text into raw bytes (the format the signing algorithm needs to work with)
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);   // STEP 4 : Signature Generation using KEY + ALGORITHM (No claims here yet). It holds the key and the name of the algorithm to be used like an instruction manual.
                                                                                            // Actual SIGNING doesn't happens here, it happens in the next step when we create the token object.
                                                                                            // Token Contents (Claims) + Key + Algorithm = SIGNED TOKEN
            var token = new JwtSecurityToken(                               // STEP 5 : Assemble the token object (Claims + Signature) but it does not perform the mathematical signing yet               
                issuer: _configuration["Jwt:Issuer"],                       // The issuer is the server that generated the token.
                audience: _configuration["Jwt:Audience"],                   // The audience is the intended recipient of the token (React/client). If the audience doesn't match, the token will be rejected.
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);         // This is where the actual signing happens. WriteToken(token) takes all the gathered information, executes the cryptographic signing math, 
        }                                                                   // And glues everything together into the final three-part JWT token string (Header.Payload.Signature) that is sent to React/client.
    }
}