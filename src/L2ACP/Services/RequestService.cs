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
    }
}