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
using System.Threading.Tasks;
using L2ACP.Extensions;
using L2ACP.Models;
using L2ACP.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;

namespace L2ACP.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IRequestService _requestService;
        private readonly IStringLocalizer<AccountController> _localizer;

        public AccountController(IAuthService authService, IRequestService requestService, IStringLocalizer<AccountController> localizer)
        {
            _authService = authService;
            _requestService = requestService;
            _localizer = localizer;
        }

        [Route("/register")]
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
                return Redirect("/");
            return View();
        }

        [Route("/logout")]
        public IActionResult Logout()
        {
            if (User.Identity.IsAuthenticated)
            {
                HttpContext.Authentication.SignOutAsync("Auth");
            }
            return RedirectToAction("Login");
        }

        [Route("/login")]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
                return Redirect("/");
            return View();
        }

        [HttpPost]
        [Route("/login")]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (User.Identity.IsAuthenticated)
                return Redirect("/");
            
            if (ModelState.IsValid)
            {
                var response = await _requestService.LoginUser(model.Username, model.Password.ToL2Password());

                if (response.ResponseCode == 200)
                {
                    await _authService.SignInUser(model.Username, HttpContext);
                    return Redirect("/");
                }
                ModelState.AddModelError(string.Empty, _localizer["Unsuccessful login"]);
            }
            return View(model);
        }

        [HttpPost]
        [Route("/register")]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (User.Identity.IsAuthenticated)
                return Redirect("/");

            if (ModelState.IsValid)
            {
                if (model.Password != model.ConfirmPassword)
                {
                    ModelState.AddModelError(string.Empty, _localizer["Passwords don't match"]);
                    return View(model);
                }

                var response = await _requestService.RegisterUser(model.Username, model.Password.ToL2Password());

                if (response.ResponseCode == 200)
                {
                    await _authService.SignInUser(model.Username, HttpContext);
                    return Redirect("/");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, _localizer["Unsuccessful registration"]);
                }
            }
            return View(model);
        }
    }
}