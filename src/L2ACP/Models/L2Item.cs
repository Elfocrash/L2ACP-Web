namespace L2ACP.Models
{
    public class L2Item
    {
        public int ItemId { get; set; }

        public string Name { get; set; }

        public string Image { get; set; }

        public bool Enchantable { get; set; }

        public bool IsQuestItem { get; set; }
    }
}