using System.Threading.Tasks;
using L2ACP.Extensions;
using L2ACP.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace L2ACP.Controllers
{
    [Route("Admin")]
    public class AdminController : Controller
    {
        private readonly IRequestService _requestService;
        public AdminController(IRequestService requestService)
        {
            _requestService = requestService;
        }

        public async Task<IActionResult> Index()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            if (HttpContext.GetAccountInfo()?.AccessLevel < 100)
                return RedirectToAction("Login", "Account");


            return View();
        }
    }
}