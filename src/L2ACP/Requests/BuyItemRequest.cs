namespace L2ACP.Requests
{
    public class BuyItemRequest : L2Request
    {
        public string AccountName { get; set; }
        public string Username { get; set; }
        public int ItemId { get; set; }
        public int ItemCount { get; set; }
        public int Enchant { get; set; }
        public int Price { get; set; }

        public BuyItemRequest() : base(9)
        {

        }
    }
}