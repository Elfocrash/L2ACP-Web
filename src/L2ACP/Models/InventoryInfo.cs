namespace L2ACP.Models
{
    public class InventoryInfo
    {
        public int ObjectId { get; set; }
        public int ItemId { get; set; }
        public int ItemCount { get; set; }
        public bool Equipped { get; set; }
        public int Enchant { get; set; }
    }
}