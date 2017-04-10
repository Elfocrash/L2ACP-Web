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
        public HomeController(IRequestService requestService)
        {
            _requestService = requestService;
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
