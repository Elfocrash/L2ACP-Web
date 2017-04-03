using System.Threading.Tasks;
using L2ACP.Extensions;
using L2ACP.Models;
using L2ACP.Repositories;
using L2ACP.Services;
using Microsoft.AspNetCore.Mvc;

namespace L2ACP.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IRequestService _requestService;

        public AccountController(IAuthService authService, IRequestService requestService)
        {
            _authService = authService;
            _requestService = requestService;
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
                ModelState.AddModelError(string.Empty, response.ResponseMessage);
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
                    ModelState.AddModelError(string.Empty, "Passwords don't match");
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
                    ModelState.AddModelError(string.Empty, response.ResponseMessage);
                }
            }
            return View(model);
        }
    }
}