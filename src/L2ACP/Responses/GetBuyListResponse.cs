using L2ACP.Models;

namespace L2ACP.Responses
{
    public class GetBuyListResponse : L2Response
    {
        public BuyListItem[] BuyList { get; set; }
    }
}