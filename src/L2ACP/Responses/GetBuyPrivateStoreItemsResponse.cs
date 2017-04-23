using L2ACP.Models;

namespace L2ACP.Responses
{
    public class GetBuyPrivateStoreItemsResponse : L2Response
    {
        public TradeItemAcp[] BuyList { get; set; }
    }
}