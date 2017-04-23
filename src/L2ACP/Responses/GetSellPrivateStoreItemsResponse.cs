using L2ACP.Models;

namespace L2ACP.Responses
{
    public class GetSellPrivateStoreItemsResponse : L2Response
    {
        public TradeItemAcp[] SellList { get; set; }
    }
}