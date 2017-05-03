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
using Microsoft.Extensions.Localization;

namespace L2ACP.Controllers
{


    [Route("admin")]
    public class AdminController : Controller
    {
        private readonly IRequestService _requestService;
        private readonly IStringLocalizer<AdminController> _localizer;

        public AdminController(IRequestService requestService, IStringLocalizer<AdminController> localizer)
        {
            _requestService = requestService;
            _localizer = localizer;
        }

        public async Task<IActionResult> Index()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            if (HttpContext.GetAccountInfo()?.AccessLevel < 100)
                return RedirectToAction("Login", "Account");

            return View();
        }

        [Route("analytics")]
        public async Task<IActionResult> Analytics()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            if (HttpContext.GetAccountInfo()?.AccessLevel < 100)
                return RedirectToAction("Login", "Account");

            var playersData = await _requestService.GetAnalyticsPlayers() as GetAnalyticsPlayersResponse;
            var viewmodel = new AnalyticsViewmodel();
            viewmodel.OnlinePlayers = playersData?.PlayerData;

            return View(viewmodel);
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
                return Content(_localizer["You need to be logged in"]);

            if (HttpContext.GetAccountInfo()?.AccessLevel < 100)
                return Content(_localizer["Not enough access level"]);

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
                return Content(_localizer["You need to be logged in"]);

            if (HttpContext.GetAccountInfo()?.AccessLevel < 100)
                return Content(_localizer["Not enough access level"]);

            var text = Request.Form["annText"];

            var response = await _requestService.AnnounceTextAsync(text);
            if (response.ResponseCode == 200)
            {
                return Content("ok:" + response.ResponseMessage);
            }
            return Content(response.ResponseMessage);
        }

        [Route("givedonatepoints")]
        [HttpPost]
        public async Task<IActionResult> GiveDonatePoints()
        {
            if (!User.Identity.IsAuthenticated)
                return Content(_localizer["You need to be logged in"]);

            if (HttpContext.GetAccountInfo()?.AccessLevel < 100)
                return Content(_localizer["Not enough access level"]);

            var playerName = Request.Form["Username"];
            var donatePoints = int.Parse(Request.Form["Points"]);

            var response = await _requestService.GiveDonatePoints(playerName, donatePoints);
            if (response.ResponseCode == 200)
            {
                return Content("ok:" + response.ResponseMessage);
            }
            return Content(response.ResponseMessage);
        }

        [Route("setplayerlevel")]
        [HttpPost]
        public async Task<IActionResult> SetPlayerLevel()
        {
            if (!User.Identity.IsAuthenticated)
                return Content(_localizer["You need to be logged in"]);

            if (HttpContext.GetAccountInfo()?.AccessLevel < 100)
                return Content(_localizer["Not enough access level"]);

            var playerName = Request.Form["Username"];
            var level = int.Parse(Request.Form["Level"]);

            var response = await _requestService.SetPlayerLevel(playerName, level);
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
                return Content(_localizer["You need to be logged in"]);

            if (HttpContext.GetAccountInfo()?.AccessLevel < 100)
                return Content(_localizer["Not enough access level"]);

            if (!int.TryParse(Request.Form["restartseconds"], out int seconds))
                return Content(_localizer["Invalid value provided"]);

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
                return Content(_localizer["You need to be logged in"]);

            if (HttpContext.GetAccountInfo()?.AccessLevel < 100)
                return Content(_localizer["Not enough access level"]);

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
                return Content(_localizer["You need to be logged in"]);

            if (HttpContext.GetAccountInfo()?.AccessLevel < 100)
                return Content(_localizer["Not enough access level"]);

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
                return Content(_localizer["You need to be logged in"]);

            if (HttpContext.GetAccountInfo()?.AccessLevel < 100)
                return Content(_localizer["Not enough access level"]);

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