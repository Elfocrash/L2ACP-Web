namespace L2ACP.Responses
{
    public class GetInventoryResponse : L2Response
    {
        public InventoryInfo[] InventoryInfo;
    }

    public class InventoryInfo
    {
        public int ItemId { get; set; }
        public int ItemCount { get; set; }
        public bool Equipped { get; set; }
        public int Enchant { get; set; }
    }
}