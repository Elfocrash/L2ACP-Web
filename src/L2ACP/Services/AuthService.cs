using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace L2ACP.Services
{
    public class AuthService : IAuthService
    {
        public async Task SignInUser(string username, HttpContext context)
        {
            var claims = new List<Claim>
                {
                    new Claim("username", username)
                };

            var id = new ClaimsIdentity(claims, "password");
            var p = new ClaimsPrincipal(id);

            await context.Authentication.SignInAsync("Auth", p);
        }
    }
}