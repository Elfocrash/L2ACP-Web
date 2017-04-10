using System.Net.Http;
using System.Threading.Tasks;
using L2ACP.Extensions;
using L2ACP.Models;
using L2ACP.Requests;
using L2ACP.Responses;
using Newtonsoft.Json;

namespace L2ACP.Services
{
    public class RequestService : IRequestService
    {
        
        public async Task<L2Response> LoginUser(string username, string password)
        {
            var loginRequest = new LoginRequest
            {
                Username = username,
                Password = password
            };
            var responseObject = await loginRequest.SendPostRequest<L2Response>();

            return responseObject;
        }

        public async Task<L2Response> RegisterUser(string username, string password)
        {
            var loginRequest = new RegisterRequest
            {
                Username = username,
                Password = password
            };
            var responseObject = await loginRequest.SendPostRequest<L2Response>();

            return responseObject;
        }

        public async Task<L2Response> GetAccountInfo(string username)
        {
            var loginRequest = new GetAccountInfoRequest
            {
                Username = username
            };

            var responseObject = await loginRequest.SendPostRequest<GetAccountInfoResponse>();

            return responseObject;
        }

        public async Task<L2Response> GetInventory(string player)
        {
            var inventoryRequest = new GetInventoryRequest()
            {
                Username = player
            };
            var responseObject = await inventoryRequest.SendPostRequest<GetInventoryResponse>();

            return responseObject;
        }

        public async Task<L2Response> GetPlayerInfo(string playerName)
        {
            var playerInfo = new GetPlayerInfoRequest()
            {
                Username = playerName
            };

            var responseObject = await playerInfo.SendPostRequest<GetPlayerInfoResponse>();

            return responseObject;
        }

        public async Task<L2Response> EnchantItem(string playerName, int objId, int itemEnch)
        {
            var enchantRequest = new EnchantItemRequest
            {
                ObjectId = objId,
                Username = playerName,
                Enchant = itemEnch
            };
            var responseObject = await enchantRequest.SendPostRequest<L2Response>();

            return responseObject;
        }

        public async Task<L2Response> SendDonation(string accountName, int amount, string transactionId, string verifySign)
        {
            var donateRequest = new DonateRequest
            {
                AccountName = accountName,
                Amount = amount,
                TransactionId = transactionId,
                VerificationSign = verifySign
            };

            var responseObject = await donateRequest.SendPostRequest<L2Response>();

            return responseObject;
        }

        public async Task<L2Response> ChangePassword(string username, string currentPass, string newPass)
        {
            var changePassRequest = new ChangePassRequest
            {
                Username = username,
                CurrentPassword = currentPass,
                NewPassword = newPass
            };

            var responseObject = await changePassRequest.SendPostRequest<L2Response>();

            return responseObject;
        }

        public async Task<L2Response> GetBuyList()
        {
            var responseObject = await new GetBuyListRequest().SendPostRequest<GetBuyListResponse>();
            return responseObject;
        }

        public async Task<L2Response> BuyItem(string accountName, string modelUsername, int modelItemId, int modelItemCount, int modelEnchant,
            int modelPrice)
        {
            var buyItemRequest = new BuyItemRequest
            {
                AccountName = accountName,
                Username = modelUsername,
                ItemId = modelItemId,
                ItemCount = modelItemCount,
                Enchant = modelEnchant,
                Price = modelPrice
            };

            var responseObject = await buyItemRequest.SendPostRequest<L2Response>();
            return responseObject;
        }

        public async Task<L2Response> GiveItem(string username, int itemId, int itemCount, int enchant)
        {
            var buyItemRequest = new GiveItemRequest
            {
                Username = username,
                ItemId = itemId,
                ItemCount = itemCount,
                Enchant = enchant
            };

            var responseObject = await buyItemRequest.SendPostRequest<L2Response>();
            return responseObject;
        }

        public async Task<L2Response> GetTopStats()
        {
            var responseObject = await new GetStatsRequest().SendPostRequest<GetStatsResponse>();
            return responseObject;
        }

        public async Task<L2Response> GetDonateServices()
        {
            var responseObject = await new GetDonateServicesRequest().SendPostRequest<GetDonateServicesResponse>();
            return responseObject;
        }

        public async Task<L2Response> RenamePlayer(string playerName, string newName)
        {
            var request = new RenamePlayerRequest
            {
                Username = playerName,
                NewName = newName
            };

            var responseObject = await request.SendPostRequest<L2Response>();
            return responseObject;
        }

        public async Task<L2Response> SetNobless(string playerName)
        {
            var request = new SetNoblessRequest
            {
                Username = playerName
            };

            var responseObject = await request.SendPostRequest<L2Response>();
            return responseObject;
        }

        public async Task<L2Response> ChangeSex(string playerName)
        {
            var request = new ChangeSexRequest
            {
                Username = playerName
            };

            var responseObject = await request.SendPostRequest<L2Response>();
            return responseObject;
        }

        public async Task<L2Response> ResetPk(string playerName)
        {
            var request = new ResetPkRequest
            {
                Username = playerName
            };

            var responseObject = await request.SendPostRequest<L2Response>();
            return responseObject;
        }

        public async Task<L2Response> GetAllPlayers()
        {
            var request = new GetAllPlayerNamesRequest();

            var responseObject = await request.SendPostRequest<GetAllPlayerNamesResponse>();
            return responseObject;
        }
    }
}