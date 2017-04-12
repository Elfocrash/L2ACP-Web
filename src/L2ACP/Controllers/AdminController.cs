using System.Linq;
using System.Threading.Tasks;
using L2ACP.Extensions;
using L2ACP.Models;
using L2ACP.Responses;
using L2ACP.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace L2ACP.Controllers
{
    [Route("admin")]
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

        [Route("acpManage")]
        public async Task<IActionResult> AcpManage()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            if (HttpContext.GetAccountInfo()?.AccessLevel < 100)
                return RedirectToAction("Login", "Account");

            return PartialView("_ACPManagment");
        }

        [Route("playerManage")]
        public async Task<IActionResult> PlayerManage()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            if (HttpContext.GetAccountInfo()?.AccessLevel < 100)
                return RedirectToAction("Login", "Account");

            var allPlayers = await _requestService.GetAllPlayers() as GetAllPlayerNamesResponse;

            return PartialView("_PlayerManagment", allPlayers?.AllPlayerNames);
        }

        [Route("serverManage")]
        public async Task<IActionResult> ServerManage()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            if (HttpContext.GetAccountInfo()?.AccessLevel < 100)
                return RedirectToAction("Login", "Account");

            return PartialView("_ServerManagment");
        }

        [Route("giveItem")]
        public async Task<IActionResult> GiveItem([FromBody] GiveItemViewmodel model)
        {
            if (!User.Identity.IsAuthenticated)
                return Content("You need to be logged in");

            if (HttpContext.GetAccountInfo()?.AccessLevel < 100)
                return Content("Oh fuck off");

            var response = await _requestService.GiveItem(model.Username, model.ItemId, model.ItemCount, model.Enchant);
            if (response.ResponseCode == 200)
            {
                return Content("ok:"+ response.ResponseMessage);
            }
            return Content(response.ResponseMessage);
        }


        [Route("announce")]
        [HttpPost]
        public async Task<IActionResult> AnnounceText()
        {
            if (!User.Identity.IsAuthenticated)
                return Content("You need to be logged in");

            if (HttpContext.GetAccountInfo()?.AccessLevel < 100)
                return Content("Oh fuck off");

            var text = Request.Form["annText"];

            var response = await _requestService.AnnounceTextAsync(text);
            if (response.ResponseCode == 200)
            {
                return Content("ok:" + response.ResponseMessage);
            }
            return Content(response.ResponseMessage);
        }
    }
}