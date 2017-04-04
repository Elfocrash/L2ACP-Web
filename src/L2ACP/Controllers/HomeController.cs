using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using L2ACP.Extensions;
using L2ACP.Responses;
using L2ACP.Services;
using Microsoft.AspNetCore.Mvc;

namespace L2ACP.Controllers
{
    [Route("/")]
    public class HomeController : Controller
    {
        private readonly IRequestService _requestService;
        public HomeController(IRequestService requestService)
        {
            _requestService = requestService;
        }

        public async Task<IActionResult> Index()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");
            IndexViewModel model = new IndexViewModel();

            var chars = await _requestService.GetAllCharNames(HttpContext.GetUsername());
            var getAllCharsResponse = chars as GetAllCharsResponse;
            if (getAllCharsResponse != null) model.CharacterNames = getAllCharsResponse.AccountNames;

            
            

            return View(model);
        }

        [HttpGet]
        [Route("getinventory/{playerName}")]
        public async Task<IActionResult> GetPlayerInventory(string playerName)
        {
            if (User.Identity.IsAuthenticated)
            {
                var chars = await _requestService.GetAllCharNames(HttpContext.GetUsername());
                var allCharsResponse = chars as GetAllCharsResponse;
                if (allCharsResponse?.ResponseCode == 200)
                {
                    if (allCharsResponse.AccountNames.Contains(playerName, StringComparer.OrdinalIgnoreCase))
                    {
                        var inventory = await _requestService.GetInventory(playerName) as GetInventoryResponse;
                        return PartialView("_Inventory", inventory);
                    }
                }
                return BadRequest();
            }
            return Unauthorized();
        }

        [HttpGet]
        [Route("getplayerinfo/{playerName}")]
        public async Task<IActionResult> GetPlayerInfo(string playerName)
        {
            if (User.Identity.IsAuthenticated)
            {
                var chars = await _requestService.GetAllCharNames(HttpContext.GetUsername());
                var allCharsResponse = chars as GetAllCharsResponse;
                if (allCharsResponse?.ResponseCode == 200)
                {
                    if (allCharsResponse.AccountNames.Contains(playerName, StringComparer.OrdinalIgnoreCase))
                    {
                        //var inventory = await _requestService.GetPlayerInfo(playerName) as GetPlayerInfoResponse;
                        return PartialView("_PlayerInfo"/*, inventory*/);
                    }
                }
                return BadRequest();
            }
            return Unauthorized();
        }

    }

    public class IndexViewModel
    {
        public IEnumerable<string> CharacterNames { get; set; }
    }
}
