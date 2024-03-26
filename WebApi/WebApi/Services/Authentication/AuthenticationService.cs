using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebApi.Interfaces;
using WebApi.Models.Models.Identity;
using WebApi.Models.Utilities;

namespace WebApi.Services.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        public IServiceProvider ServiceProvider { get; set; }

        public AuthenticationService(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public string AuthenticateMachine(string appSecret)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(appSecret);

            var tokeDescriptor = new SecurityTokenDescriptor
            {
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Subject = new ClaimsIdentity(new[] 
                {
                    new Claim(ClaimTypes.Role, IdentityData.MachineClaimName) 
                }),
                Expires = DateTime.UtcNow.AddMinutes(15)
            };

            var token = tokenHandler.CreateToken(tokeDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public async Task<string> AuthenticateUser(long userId, string appSecret)
        { 
            var userRepo = ServiceProvider.GetRequiredService<IUserRepository>();

            var user = await userRepo.GetUserAsync(userId);
            if (user == null)
                return null;

            if(!user.IsAdmin && !user.IsSponsor)
                return null;

            // If user is admin but has no claims
            var userRoles = await userRepo.GetUserRolesAsync(userId);
            if (userRoles.Count == 0)
                return null;

            // Assign roles
            var tokenRoles = new List<Claim>();
            foreach (var role in userRoles)
            {
                tokenRoles.Add(new Claim(ClaimTypes.Role, role.Role.ToLowerString()));
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(appSecret);

            // Set roles based on db data here
            var tokeDescriptor = new SecurityTokenDescriptor
            {
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Subject = new ClaimsIdentity(tokenRoles),
                Expires = DateTime.Now
            };

            var token = tokenHandler.CreateToken(tokeDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
