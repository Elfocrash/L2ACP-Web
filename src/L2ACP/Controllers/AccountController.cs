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
        private readonly IRepository _repository;

        public AccountController(IAuthService authService, IRepository repository)
        {
            _authService = authService;
            _repository = repository;
        }

        [Route("/register")]
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
                return Redirect("/");
            return View();
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
                if (await _repository.IsValidLogin(model.Username, model.Password.ToL2Password()))
                {
                    await _authService.SignInUser(model.Username, HttpContext);
                    return Redirect("/");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
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

                if (await _repository.IsValidRegister(model.Username))
                {
                    await _repository.RegisterUser(model.Username, model.Password.ToL2Password());
                    await _authService.SignInUser(model.Username, HttpContext);
                    return Redirect("/");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Account already exists with that name.");
                }
            }
            return View(model);
        }
    }
}