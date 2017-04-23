namespace L2ACP.Requests
{
    public class SellPrivateStoreItemRequest : L2Request
    {
        public int ObjectId { get; set; }
        public int BuyerId { get; set; }
        public int Count { get; set; }
        public string SellerName { get; set; }

        public SellPrivateStoreItemRequest() : base(33)
        {
        }
    }
}