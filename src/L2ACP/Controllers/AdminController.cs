using System;
using System.Collections.Generic;
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

            var getBuyList = await _requestService.GetBuyList() as GetBuyListResponse;
            var getServiceList = await _requestService.GetDonateServices() as GetDonateServicesResponse;

            var viewModel = new AcpManageViewmodel
            {
                BuyListItems = getBuyList?.BuyList,
                DonateServices = getServiceList?.DonateServices
            };
            return PartialView("_ACPManagment", viewModel);
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

        [Route("liveMap")]
        public async Task<IActionResult> LiveMap()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            if (HttpContext.GetAccountInfo()?.AccessLevel < 100)
                return RedirectToAction("Login", "Account");

            return PartialView("_LiveMap");
        }

        [Route("getLiveMapData")]
        public async Task<IActionResult> LiveMapData()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            if (HttpContext.GetAccountInfo()?.AccessLevel < 100)
                return RedirectToAction("Login", "Account");

            var response = await _requestService.GetAllOnlinePlayersForMap() as GetAllOnlinePlayersForMapResponse;

            if (response != null && response.ResponseCode == 200)
            {
                var players = response.MapPlayers;
                foreach (var player in players)
                {
                    player.X = (int) Math.Round((double)(116 + (player.X + 107823) / 200));
                    player.Y = (int)Math.Round((double)(2580 + (player.Y - 255420) / 200));
                }

                return new JsonResult(players);
            }
            return BadRequest();

          
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

        [Route("serverrestart")]
        [HttpPost]
        public async Task<IActionResult> ServerRestart()
        {
            if (!User.Identity.IsAuthenticated)
                return Content("You need to be logged in");

            if (HttpContext.GetAccountInfo()?.AccessLevel < 100)
                return Content("Oh fuck off");

            if (!int.TryParse(Request.Form["restartseconds"], out int seconds))
                return Content("Invalid value provided");

            var response = await _requestService.RestartServer(seconds);
            if (response.ResponseCode == 200)
            {
                return Content("ok:" + response.ResponseMessage);
            }
            return Content(response.ResponseMessage);
        }

        [Route("setdonatelist")]
        [HttpPost]
        public async Task<IActionResult> SetDonateList([FromBody]AdminDonateListViewmodel[] items)
        {
            if (!User.Identity.IsAuthenticated)
                return Content("You need to be logged in");

            if (HttpContext.GetAccountInfo()?.AccessLevel < 100)
                return Content("Oh fuck off");

            var response = await _requestService.SetDonateList(items);
            if (response.ResponseCode == 200)
            {
                return Content("ok:" + response.ResponseMessage);
            }
            return Content(response.ResponseMessage);
        }

        [Route("spawnnpc")]
        [HttpPost]
        public async Task<IActionResult> SpawnNpc()
        {
            if (!User.Identity.IsAuthenticated)
                return Content("You need to be logged in");

            if (HttpContext.GetAccountInfo()?.AccessLevel < 100)
                return Content("Oh fuck off");

            var npcId = int.Parse(Request.Form["NpcId"]);
            var x = int.Parse(Request.Form["X"]);
            var y = int.Parse(Request.Form["Y"]);

            var response = await _requestService.SpawnNpc(npcId,x,y);
            if (response.ResponseCode == 200)
            {
                return Content("ok:" + response.ResponseMessage);
            }
            return Content(response.ResponseMessage);
        }

        [Route("punish")]
        [HttpPost]
        public async Task<IActionResult> Punish()
        {
            if (!User.Identity.IsAuthenticated)
                return Content("You need to be logged in");

            if (HttpContext.GetAccountInfo()?.AccessLevel < 100)
                return Content("Oh fuck off");

            var punishId = int.Parse(Request.Form["PunishId"]);
            var playerName = Request.Form["PlayerName"];
            var time = int.Parse(Request.Form["Time"]);

            var response = await _requestService.Punish(punishId, playerName, time);
            if (response.ResponseCode == 200)
            {
                return Content("ok:" + response.ResponseMessage);
            }
            return Content(response.ResponseMessage);
        }
    }
}