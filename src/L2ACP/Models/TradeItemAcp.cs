namespace L2ACP.Models
{
    public class TradeItemAcp
    {
        public int ObjectId { get; set; }
        public int ItemId { get; set; }
        public int Enchant { get; set; }
        public int Count { get; set; }
        public int Price { get; set; }
        public string PlayerName { get; set; }
        public int PlayerId { get; set; }
    }
}