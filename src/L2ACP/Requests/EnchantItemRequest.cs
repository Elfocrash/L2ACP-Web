namespace L2ACP.Requests
{
    public class EnchantItemRequest : L2Request
    {
        public string Username { get; set; }
        public int ObjectId { get; set; }
        public int Enchant { get; set; }

        public EnchantItemRequest() : base(6)
        {

        }
    }
}