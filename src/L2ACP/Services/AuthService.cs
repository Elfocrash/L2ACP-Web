/*
 * Copyright (C) 2017  Nick Chapsas
 * This program is free software: you can redistribute it and/or modify it under
 * the terms of the GNU General Public License as published by the Free Software
 * Foundation, either version 2 of the License, or (at your option) any later
 * version.
 * 
 * L2ACP is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
 * FOR A PARTICULAR PURPOSE. See the GNU General Public License for more
 * details.
 * 
 * You should have received a copy of the GNU General Public License along with
 * this program. If not, see <http://www.gnu.org/licenses/>.
 */
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
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

            await context.SignInAsync("Auth", p);
        }
    }
}