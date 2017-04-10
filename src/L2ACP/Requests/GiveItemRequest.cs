namespace L2ACP.Requests
{
    public class GiveItemRequest : L2Request
    {
        public string Username { get; set; }
        public int ItemId { get; set; }
        public int ItemCount { get; set; }
        public int Enchant { get; set; }

        public GiveItemRequest() : base(18)
        {

        }
    }
}