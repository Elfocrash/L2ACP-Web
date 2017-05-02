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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using L2ACP.Extensions;
using L2ACP.Models;
using L2ACP.Responses;
using L2ACP.Services;
using Microsoft.AspNetCore.Mvc;

namespace L2ACP.Controllers
{
    [Route("/")]
    public class HomeController : Controller
    {
        private readonly IRequestService _requestService;
        private readonly AssetManager _assetManager;
        public HomeController(IRequestService requestService, AssetManager assetManager)
        {
            _requestService = requestService;
            _assetManager = assetManager;
        }

        public async Task<IActionResult> Index()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");
            IndexViewModel model = new IndexViewModel();

            var allCharsResponse = HttpContext.GetAccountInfo();
            if (allCharsResponse == null)
                return BadRequest();
            model.CharacterNames = allCharsResponse.AccountNames;
            return View(model);
        }

        [Route("/services")]
        public async Task<IActionResult> Services()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");
            IndexViewModel model = new IndexViewModel();

            var allCharsResponse = HttpContext.GetAccountInfo();
            if (allCharsResponse == null)
                return BadRequest();

            model.CharacterNames = allCharsResponse.AccountNames;
            return View(model);
        }

        [Route("/raidbossmap")]
        public async Task<IActionResult> RbMap()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");
            var allCharsResponse = HttpContext.GetAccountInfo();
            if (allCharsResponse == null)
                return BadRequest();
            return View();
        }

        [Route("getRbData")]
        public async Task<IActionResult> LiveMapData()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");
            var allCharsResponse = HttpContext.GetAccountInfo();
            if (allCharsResponse == null)
                return BadRequest();

            var response = await _requestService.GetAllBossesForMap() as GetLiveRbsForMapResponse;

            if (response != null && response.ResponseCode == 200)
            {
                var mobs = response.MapMobs;
                foreach (var mob in mobs)
                {
                    mob.X = (int)Math.Round((double)(116 + (mob.X + 107823) / 200));
                    mob.Y = (int)Math.Round((double)(2580 + (mob.Y - 255420) / 200));
                }

                return new JsonResult(mobs);
            }
            return BadRequest();
        }


        [Route("/luckywheel")]
        public async Task<IActionResult> LuckyWheel()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var allCharsResponse = HttpContext.GetAccountInfo();
            if (allCharsResponse == null)
                return BadRequest();

            LuckyWheelViewmodel model = new LuckyWheelViewmodel();
            var wheelItems = await _requestService.GetLuckyWheelList() as LuckyWheelListResponse;

            model.Items = wheelItems?.Items.ToList();
            model.CharacterNames = allCharsResponse.AccountNames;

            return View(model);
        }

        [Route("/statistics")]
        public async Task<IActionResult> Statistics()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");


            var stats = await _requestService.GetTopStats() as GetStatsResponse;
            if (stats != null)
            {
                StatsViewModel model = new StatsViewModel
                {
                    TopPvp = stats.TopPvp,
                    TopPk = stats.TopPk,
                    TopOnline = stats.TopOnline
                };

                return View(model);
            }
            return View();
        }

        [Route("/getservices")]
        public IActionResult GetServices()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            return PartialView("_ServiceItems");
        }

        [HttpGet]
        [Route("getinventory/{playerName}")]
        public async Task<IActionResult> GetPlayerInventory(string playerName)
        {
            if (User.Identity.IsAuthenticated)
            {
                var allCharsResponse = HttpContext.GetAccountInfo();
                if (allCharsResponse == null)
                    return BadRequest();

                if (allCharsResponse.AccountNames.Contains(playerName, StringComparer.OrdinalIgnoreCase))
                {
                    var inventory = await _requestService.GetInventory(playerName) as GetInventoryResponse;
                    return PartialView("_Inventory", inventory);
                }
                
                return BadRequest();
            }
            return Unauthorized();
        }

        [HttpGet]
        [Route("renameplayer/{playerName}/{newName}")]
        public async Task<IActionResult> RenamePlayer(string playerName, string newName)
        {
            if (User.Identity.IsAuthenticated)
            {
                var allCharsResponse = HttpContext.GetAccountInfo();
                if (allCharsResponse == null)
                    return BadRequest();

                if (allCharsResponse.AccountNames.Contains(playerName, StringComparer.OrdinalIgnoreCase))
                {
                    var response = await _requestService.RenamePlayer(playerName, newName);
                    if(response.ResponseCode == 200)
                        return Content("ok:" + response.ResponseMessage);
                    return Content(response.ResponseMessage);
                }

                return BadRequest();
            }
            return Unauthorized();
        }

        [HttpGet]
        [Route("setnobless/{playerName}")]
        public async Task<IActionResult> SetNobless(string playerName)
        {
            if (User.Identity.IsAuthenticated)
            {
                var allCharsResponse = HttpContext.GetAccountInfo();
                if (allCharsResponse == null)
                    return BadRequest();

                if (allCharsResponse.AccountNames.Contains(playerName, StringComparer.OrdinalIgnoreCase))
                {
                    var response = await _requestService.SetNobless(playerName);
                    if (response.ResponseCode == 200)
                        return Content("ok:" + response.ResponseMessage);
                    return Content(response.ResponseMessage);
                }

                return BadRequest();
            }
            return Unauthorized();
        }

        [Route("luckyspin")]
        [HttpPost]
        public async Task<IActionResult> LuckySpin()
        {
            if (User.Identity.IsAuthenticated)
            {
                var allCharsResponse = HttpContext.GetAccountInfo();
                if (allCharsResponse == null)
                    return BadRequest();

                var playerName = Request.Form["char_name"].ToString();

                if (allCharsResponse.AccountNames.Contains(playerName, StringComparer.OrdinalIgnoreCase))
                {
                    var response = await _requestService.SpinLuckyWheel(playerName) as LuckyWheelSpinResponse;
                    if (response != null && response.ResponseCode == 200)
                    {
                        var winItem = response.Item;
                        L2Item item = _assetManager.GetItems()[winItem.ItemId];

                        return Json(new
                        {
                            status = "success",
                            ItemImg = item.Image,
                            ItemName = item.Name,
                            Count = winItem.Count
                        });
                    }
                        
                    return Json(new {status = "error", message = response?.ResponseMessage});
                }
                return Json(new { status = "error", message = "This is not your character" });
            }
            return Unauthorized();
        }

        [HttpGet]
        [Route("changeSex/{playerName}")]
        public async Task<IActionResult> ChangeSex(string playerName)
        {
            if (User.Identity.IsAuthenticated)
            {
                var allCharsResponse = HttpContext.GetAccountInfo();
                if (allCharsResponse == null)
                    return BadRequest();

                if (allCharsResponse.AccountNames.Contains(playerName, StringComparer.OrdinalIgnoreCase))
                {
                    var response = await _requestService.ChangeSex(playerName);
                    if (response.ResponseCode == 200)
                        return Content("ok:" + response.ResponseMessage);
                    return Content(response.ResponseMessage);
                }

                return BadRequest();
            }
            return Unauthorized();
        }

        [HttpGet]
        [Route("resetPk/{playerName}")]
        public async Task<IActionResult> ResetPk(string playerName)
        {
            if (User.Identity.IsAuthenticated)
            {
                var allCharsResponse = HttpContext.GetAccountInfo();
                if (allCharsResponse == null)
                    return BadRequest();

                if (allCharsResponse.AccountNames.Contains(playerName, StringComparer.OrdinalIgnoreCase))
                {
                    var response = await _requestService.ResetPk(playerName);
                    if (response.ResponseCode == 200)
                        return Content("ok:" + response.ResponseMessage);
                    return Content(response.ResponseMessage);
                }

                return BadRequest();
            }
            return Unauthorized();
        }

        [HttpGet]
        [Route("getenchantableitems/{playerName}")]
        public async Task<IActionResult> GetEnchantableItems(string playerName)
        {
            if (User.Identity.IsAuthenticated)
            {
                var allCharsResponse = HttpContext.GetAccountInfo();
                if (allCharsResponse == null)
                    return BadRequest();

                if (allCharsResponse.AccountNames.Contains(playerName, StringComparer.OrdinalIgnoreCase))
                {
                    var inventory = await _requestService.GetInventory(playerName) as GetInventoryResponse;
                    return PartialView("_EnchantableItems", inventory);
                }
                
                return BadRequest();
            }
            return Unauthorized();
        }

        [HttpGet]
        [Route("getbuylist")]
        public async Task<IActionResult> GetBuyList()
        {
            if (User.Identity.IsAuthenticated)
            {
                var buyList = await _requestService.GetBuyList() as GetBuyListResponse;
                if(buyList != null)
                    return PartialView("_BuyList", buyList.BuyList);
                return BadRequest();
            }
            return Unauthorized();
        }

        [HttpGet]
        [Route("buyprivatestores")]
        public async Task<IActionResult> BuyPrivateStores()
        {
            if (User.Identity.IsAuthenticated)
            {
                var buyList = await _requestService.GetBuyPrivateStoreList() as GetBuyPrivateStoreItemsResponse;
                if (buyList != null)
                {
                    ViewBag.CharacterNames = HttpContext.GetAccountInfo().AccountNames;
                    return View("BuyPrivateStore", buyList.BuyList);
                }
                return BadRequest();
            }
            return Unauthorized();
        }


        [Route("sellitemprivate")]
        [HttpPost]
        public async Task<IActionResult> SellItemPrivate()
        {
            if (User.Identity.IsAuthenticated)
            {
                var allCharsResponse = HttpContext.GetAccountInfo();
                if (allCharsResponse == null)
                    return BadRequest();

                var playerName = Request.Form["SellerName"].ToString();
                if (allCharsResponse.AccountNames.Contains(playerName, StringComparer.OrdinalIgnoreCase))
                {
                    var objectId = int.Parse(Request.Form["ObjectId"]);
                    var sellerId = int.Parse(Request.Form["BuyerId"]);
                    var count = int.Parse(Request.Form["Count"]);
                    var buyerName = Request.Form["SellerName"].ToString();

                    var response = await _requestService.SellPrivateStoreItem(objectId, sellerId, count,buyerName);
                    if (response.ResponseCode == 200)
                    {
                        return Content("ok:" + response.ResponseMessage);
                    }
                    return Content(response.ResponseMessage);
                }
                return BadRequest();
            }
            return Unauthorized();
        }


        [HttpGet]
        [Route("sellprivatestores")]
        public async Task<IActionResult> SellPrivateStores()
        {
            if (User.Identity.IsAuthenticated)
            {
                var buyList = await _requestService.GetSellPrivateStoreList() as GetSellPrivateStoreItemsResponse;
                if (buyList != null)
                {
                    ViewBag.CharacterNames = HttpContext.GetAccountInfo().AccountNames;
                    return View("SellPrivateStore", buyList.SellList);
                }
                    
                return BadRequest();
            }
            return Unauthorized();
        }

        [Route("buyitemprivate")]
        [HttpPost]
        public async Task<IActionResult> BuyItemPrivate()
        {
            if (User.Identity.IsAuthenticated)
            {
                var allCharsResponse = HttpContext.GetAccountInfo();
                if (allCharsResponse == null)
                    return BadRequest();

                var playerName = Request.Form["BuyerName"].ToString();
                if (allCharsResponse.AccountNames.Contains(playerName, StringComparer.OrdinalIgnoreCase))
                {
                    var objectId = int.Parse(Request.Form["ObjectId"]);
                    var buyerId = int.Parse(Request.Form["SellerId"]);
                    var count = int.Parse(Request.Form["Count"]);
                    var sellerName = Request.Form["BuyerName"].ToString();

                    var response = await _requestService.BuyPrivateStoreItem(objectId, buyerId, count, sellerName);
                    if (response.ResponseCode == 200)
                    {
                        return Content("ok:" + response.ResponseMessage);
                    }
                    return Content(response.ResponseMessage);
                }
                return BadRequest();
            }
            return Unauthorized();
        }

        [HttpGet]
        [Route("getdonateservices")]
        public async Task<IActionResult> GetDonateServices()
        {
            if (User.Identity.IsAuthenticated)
            {
                var buyList = await _requestService.GetDonateServices() as GetDonateServicesResponse;
                if (buyList != null)
                    return PartialView("_DonateServices", buyList.DonateServices);
                return BadRequest();
            }
            return Unauthorized();
        }

        [HttpPost]
        [Route("buyitem")]
        public async Task<IActionResult> BuyItem([FromBody] BuyItemViewmodel model)
        {
            if (User.Identity.IsAuthenticated)
            {
                var accountName = HttpContext.GetUsername();

                var response = await _requestService.BuyItem(accountName, model.Username,model.ItemId,model.ItemCount,model.Enchant,model.Price);
                if (response.ResponseCode == 200)
                {
                    return Content("paid:" + model.Price);
                }
                return Content(response.ResponseMessage);

            }
            return Unauthorized();
        }

        [HttpPost]
        [Route("changepass")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePassViewmodel model)
        {
            if (User.Identity.IsAuthenticated)
            {
                var accountName = HttpContext.GetUsername();

                var response = await _requestService.ChangePassword(accountName, model.CurrentPassword.ToL2Password(), model.NewPassword.ToL2Password());
                if (response.ResponseCode == 200)
                {
                    return Content("ok");
                }
                return Content(response.ResponseMessage);

            }
            return Unauthorized();
        }

        //[HttpGet]
        //[Route("enchantitem/{playerName}/{objId}")]
        //public async Task<IActionResult> GetEnchantableItems(string playerName, int objId)
        //{
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        var allCharsResponse = HttpContext.GetAccountInfo();
        //        if (allCharsResponse == null)
        //            return BadRequest();
        //        if (allCharsResponse.AccountNames.Contains(playerName, StringComparer.OrdinalIgnoreCase))
        //            {
        //                var inventory = await _requestService.GetInventory(playerName) as GetInventoryResponse;
        //                var contains = inventory?.InventoryInfo.Select(x => x.ObjectId).Contains(objId);
        //                if (contains != null && (bool) contains)
        //                {
        //                    var itemEnch = inventory.InventoryInfo.FirstOrDefault(x => x.ObjectId == objId).Enchant;
        //                    var enchantResponse = await _requestService.EnchantItem(playerName, objId, itemEnch) as L2Response;
        //                    if (enchantResponse.ResponseCode == 200)
        //                    {
        //                        return Content("ok");
        //                    }
        //                    return BadRequest();
        //                }
        //                else
        //                {
        //                    return BadRequest();
        //                }
                        
                    
        //        }
        //        return BadRequest();
        //    }
        //    return Unauthorized();
        //}

        [HttpGet]
        [Route("getplayerinfo/{playerName}")]
        public async Task<IActionResult> GetPlayerInfo(string playerName)
        {
            if (User.Identity.IsAuthenticated)
            {
                var allCharsResponse = HttpContext.GetAccountInfo();
                if (allCharsResponse == null)
                    return BadRequest();

                if (allCharsResponse.AccountNames.Contains(playerName, StringComparer.OrdinalIgnoreCase))
                {
                    var playerInfo = await _requestService.GetPlayerInfo(playerName) as GetPlayerInfoResponse;
                    return PartialView("_PlayerInfo", playerInfo);
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
