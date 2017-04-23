namespace L2ACP.Requests
{
    public class BuyPrivateStoreItemRequest : L2Request
    {
        public int ObjectId { get; set; }
        public int SellerId { get; set; }
        public int Count { get; set; }
        public string BuyerName { get; set; }

        public BuyPrivateStoreItemRequest() : base(34)
        {
        }
    }
}