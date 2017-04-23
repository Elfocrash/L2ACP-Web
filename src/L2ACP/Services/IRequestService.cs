using System.Threading.Tasks;
using L2ACP.Models;
using L2ACP.Responses;
using Microsoft.Extensions.Primitives;

namespace L2ACP.Services
{
    public interface IRequestService
    {
        Task<L2Response> LoginUser(string username, string password);

        Task<L2Response> RegisterUser(string username, string password);

        Task<L2Response> ChangePassword(string username, string currentPass, string newPass);

        Task<L2Response> GetAccountInfo(string getUsername);

        Task<L2Response> GetInventory(string player);

        Task<L2Response> GetPlayerInfo(string playerName);

        Task<L2Response> EnchantItem(string playerName, int objId, int itemEnch);

        Task<L2Response> SendDonation(string accountName, int amount, string transactionId, string verifySign);

        Task<L2Response> GetBuyList();

        Task<L2Response> BuyItem(string accountName, string modelUsername, int modelItemId, int modelItemCount, int modelEnchant, int modelPrice);

        Task<L2Response> GetTopStats();

        Task<L2Response> GetDonateServices();

        Task<L2Response> RenamePlayer(string playerName, string newName);

        Task<L2Response> SetNobless(string playerName);

        Task<L2Response> ChangeSex(string playerName);

        Task<L2Response> ResetPk(string playerName);

        Task<L2Response> GetAllPlayers();

        Task<L2Response> GiveItem(string username, int itemId, int itemCount, int enchant);

        Task<L2Response> AnnounceTextAsync(string text);

        Task<L2Response> Punish(int punishId, string playerName, int time);

        Task<L2Response> GetAllOnlinePlayersForMap();

        Task<L2Response> SpawnNpc(int npcId, int x, int y);

        Task<L2Response> SetDonateList(AdminDonateListViewmodel[] items);

        Task<L2Response> RestartServer(int seconds);

        Task<L2Response> GetLuckyWheelList();

        Task<L2Response> SpinLuckyWheel(string playername);

        Task<L2Response> GetAnalyticsPlayers();

        Task<L2Response> GetAllBossesForMap();

        Task<L2Response> GiveDonatePoints(string playerName, int donatePoints);

        Task<L2Response> SetPlayerLevel(string playerName, int level);

        Task<L2Response> GetBuyPrivateStoreList();

        Task<L2Response> GetSellPrivateStoreList();

        Task<L2Response> SellPrivateStoreItem(int objectId, int sellerId, int count, string buyerName);

        Task<L2Response> BuyPrivateStoreItem(int objectId, int buyerId, int count, string sellerName);
    }
}